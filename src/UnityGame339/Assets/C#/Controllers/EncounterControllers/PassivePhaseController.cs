using System;
using System.Collections;
using Game339.Shared.DependencyInjection;
using UnityEngine;

/// <summary>
/// At the start of each player turn, processes leg passives for the player and the enemy
/// (Block / Regen / Thorns). Plays a small delay between effects so each is visible.
///
/// Fires OnPassivePhaseStart at the beginning and OnPassivePhaseComplete at the end —
/// PlayerController listens for Complete to enable input; TurnIndicatorView listens for
/// Start to swap the label to "Passive Phase".
///
/// If neither side has any active passives this turn, both events are skipped entirely
/// EXCEPT OnPassivePhaseComplete (which always fires so input gating still works).
/// </summary>
public class PassivePhaseController : EncounterController
{
    [Tooltip("Seconds of delay between each passive trigger so each is visible.")]
    [SerializeField] private float _stepDelay = 0.4f;

    public event Action OnPassivePhaseStart;
    public event Action OnPassivePhaseComplete;

    protected override void EncounterBegin()
    {
        _turnEngine.PlayerTurnStart += OnPlayerTurnStart;
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _turnEngine.PlayerTurnStart -= OnPlayerTurnStart;
    }

    private void OnPlayerTurnStart()
    {
        if (HasAnyPassivesToProcess())
        {
            OnPassivePhaseStart?.Invoke();
            StartCoroutine(RunPassivePhase());
        }
        else
        {
            // No passives — skip the visible phase entirely, fire complete so PlayerController proceeds.
            OnPassivePhaseComplete?.Invoke();
        }
    }

    private IEnumerator RunPassivePhase()
    {
        if (Player != null) yield return RunPassivesFor(Player);
        if (Enemy != null) yield return RunPassivesFor(Enemy);
        OnPassivePhaseComplete?.Invoke();
    }

    private IEnumerator RunPassivesFor(Character owner)
    {
        BodyPart[] legs = owner.Loadout != null ? owner.Loadout.legs : null;
        if (legs == null) yield break;

        foreach (BodyPart leg in legs)
        {
            if (leg == null || leg.passiveType == PassiveType.None) continue;
            if (owner.HasDied) yield break;

            ApplyPassive(owner, leg);
            yield return new WaitForSeconds(_stepDelay);
        }
    }

    private bool HasAnyPassivesToProcess()
    {
        return HasAnyPassives(Player) || HasAnyPassives(Enemy);
    }

    private static bool HasAnyPassives(Character owner)
    {
        if (owner == null || owner.Loadout == null) return false;
        BodyPart[] legs = owner.Loadout.legs;
        if (legs == null) return false;
        foreach (BodyPart leg in legs)
        {
            if (leg != null && leg.passiveType != PassiveType.None) return true;
        }
        return false;
    }

    private static void ApplyPassive(Character owner, BodyPart leg)
    {
        switch (leg.passiveType)
        {
            case PassiveType.Block:
                owner.AddBlock(leg.passiveValue);
                break;
            case PassiveType.Regen:
                owner.Heal(leg.passiveValue);
                break;
            case PassiveType.Thorns:
                owner.ApplyThorns(leg.passiveValue);
                break;
        }
    }
}