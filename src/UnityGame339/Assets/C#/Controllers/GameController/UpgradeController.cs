using System.Collections.Generic;
using Game.Runtime;
using Game339.Shared;
using Game339.Shared.DependencyInjection;
using UnityEngine;

/// <summary>
/// Drives the new body-part upgrade flow:
///   EncounterEnd (win) → pick random arm + leg drops from defeated enemy →
///   show upgrade screen → player picks an offer → player picks a slot →
///   confirmation modal → confirm commits, back returns → BeginNewEncounter.
/// </summary>
public class UpgradeController : GameController
{
    [SerializeField] private UpgradeView _upgradeView;

    /// <summary>Observed by UpgradeView to show/hide its panel.</summary>
    public ObservableValue<bool> IsUpgradeAvailable { get; } = new ObservableValue<bool>();

    private Character _lastEnemy;

    // Offer + selection state (controller-internal — view no longer reflects it visually).
    private BodyPart _offeredArm;
    private BodyPart _offeredLeg;
    private BodyPart _selectedOffer;
    private BodyPartType _selectedOfferType;
    private int _replacingSlotIndex;

    public Character GetPlayer() => Player;

    protected override void Subscribe()
    {
        _turnEngine.EncounterSetup += OnEncounterSetup;
        _turnEngine.EncounterEnd += OnEncounterEnd;
        _upgradeView.Initialize(this);
    }

    protected override void Unsubscribe()
    {
        _turnEngine.EncounterSetup -= OnEncounterSetup;
        _turnEngine.EncounterEnd -= OnEncounterEnd;
        _upgradeView.Deinitialize();
    }

    //===== Encounter lifecycle =====

    private void OnEncounterSetup(ICharacter player, ICharacter enemy)
    {
        _lastEnemy = enemy as Character;
    }

    private void OnEncounterEnd(bool playerWon)
    {
        if (!playerWon) return;
        if (_lastEnemy == null)
        {
            Debug.LogWarning("UpgradeController: no enemy reference on EncounterEnd — skipping upgrade.");
            return;
        }

        _offeredArm = PickRandomNonNull(_lastEnemy.Loadout.arms);
        _offeredLeg = PickRandomNonNull(_lastEnemy.Loadout.legs);

        if (_offeredArm == null && _offeredLeg == null)
        {
            Debug.LogWarning("UpgradeController: enemy had no parts to drop — skipping upgrade.");
            EncounterManager.Instance.BeginNewEncounter();
            return;
        }

        _selectedOffer = null;

        _upgradeView.DisplayOffers(_offeredArm, _offeredLeg);
        _upgradeView.PopulateCreaturePreview(Player);
        _upgradeView.DisableAllSlots();
        _upgradeView.HideConfirmation();
        _upgradeView.ClearStatsPreview();

        IsUpgradeAvailable.Value = true;
    }

    private static BodyPart PickRandomNonNull(BodyPart[] source)
    {
        if (source == null || source.Length == 0) return null;
        List<BodyPart> valid = new List<BodyPart>();
        foreach (BodyPart p in source) if (p != null) valid.Add(p);
        if (valid.Count == 0) return null;
        return valid[Random.Range(0, valid.Count)];
    }

    //===== Player actions (called by view) =====

    public void OnOfferHoverEnter(BodyPart offer)
    {
        if (offer == null) return;
        var (maxHP, attack) = ComputeOptimalPreview(offer);
        _upgradeView.UpdateStatsPreview(maxHP, attack);
    }

    public void OnOfferHoverExit()
    {
        if (_selectedOffer != null)
        {
            var (maxHP, attack) = ComputeOptimalPreview(_selectedOffer);
            _upgradeView.UpdateStatsPreview(maxHP, attack);
        }
        else
        {
            _upgradeView.ClearStatsPreview();
        }
    }

    public void OnOfferClicked(BodyPart offer)
    {
        if (offer == null) return;

        _selectedOffer = offer;
        _selectedOfferType = offer.partType;

        _upgradeView.DisableAllSlots();
        _upgradeView.EnableSlotsForType(_selectedOfferType, true);

        var (maxHP, attack) = ComputeOptimalPreview(offer);
        _upgradeView.UpdateStatsPreview(maxHP, attack);
    }

    public void OnSlotClicked(BodyPartType type, int index)
    {
        if (_selectedOffer == null) return;
        if (type != _selectedOfferType) return;

        _replacingSlotIndex = index;

        BodyPart[] slots = GetSlotsForType(type);
        if (index < 0 || index >= slots.Length) return;
        BodyPart oldPart = slots[index];

        _upgradeView.ShowConfirmation(oldPart, _selectedOffer);
        _upgradeView.DisableAllSlots();

        var (maxHP, attack) = ComputeSwapResult(_selectedOffer, type, index);
        _upgradeView.UpdateStatsPreview(maxHP, attack);
    }

    public void OnConfirm()
    {
        if (_selectedOffer == null) return;

        BodyPart[] slots = GetSlotsForType(_selectedOfferType);
        slots[_replacingSlotIndex] = _selectedOffer;

        Player.RecomputeStats();
        Player.HealToFull();

        _selectedOffer = null;
        _offeredArm = null;
        _offeredLeg = null;

        _upgradeView.HideConfirmation();
        _upgradeView.ClearStatsPreview();
        IsUpgradeAvailable.Value = false;

        EncounterManager.Instance.BeginNewEncounter();
    }

    public void OnBack()
    {
        _upgradeView.HideConfirmation();
        _upgradeView.EnableSlotsForType(_selectedOfferType, true);

        if (_selectedOffer != null)
        {
            var (maxHP, attack) = ComputeOptimalPreview(_selectedOffer);
            _upgradeView.UpdateStatsPreview(maxHP, attack);
        }
        else
        {
            _upgradeView.ClearStatsPreview();
        }
    }

    //===== Stat preview math =====

    private (int maxHP, int attack) ComputeOptimalPreview(BodyPart offer)
    {
        if (offer == null) return (Player.MaxHP.Value, Player.Attack.Value);

        BodyPart[] slots = GetSlotsForType(offer.partType);
        int bestMaxHP = Player.MaxHP.Value;
        int bestAttack = Player.Attack.Value;
        int bestScore = int.MinValue;
        bool foundAny = false;

        for (int i = 0; i < slots.Length; i++)
        {
            var (newMaxHP, newAttack) = ComputeSwapResult(offer, offer.partType, i);
            int score = newMaxHP + newAttack;
            if (!foundAny || score > bestScore)
            {
                bestScore = score;
                bestMaxHP = newMaxHP;
                bestAttack = newAttack;
                foundAny = true;
            }
        }

        return (bestMaxHP, bestAttack);
    }

    private (int maxHP, int attack) ComputeSwapResult(BodyPart newPart, BodyPartType type, int slotIndex)
    {
        BodyPart[] slots = GetSlotsForType(type);
        if (slotIndex < 0 || slotIndex >= slots.Length) return (Player.MaxHP.Value, Player.Attack.Value);

        BodyPart current = slots[slotIndex];
        int currentMaxHPMod = current != null ? current.maxHPModifier : 0;
        int currentAttackMod = current != null ? current.attackModifier : 0;
        int newMaxHPMod = newPart != null ? newPart.maxHPModifier : 0;
        int newAttackMod = newPart != null ? newPart.attackModifier : 0;

        int newMaxHP = Player.MaxHP.Value - currentMaxHPMod + newMaxHPMod;
        int newAttack = Player.Attack.Value - currentAttackMod + newAttackMod;
        return (newMaxHP, newAttack);
    }

    private BodyPart[] GetSlotsForType(BodyPartType type)
    {
        return (type == BodyPartType.Arm) ? Player.Loadout.arms : Player.Loadout.legs;
    }
}