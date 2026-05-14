using System;
using Game339.Shared;
using Game339.Shared.DependencyInjection;
using Game339.Shared.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Character")]
public class Character : ScriptableObject, ICharacter
{
    [SerializeField] private Sprite _characterIcon;
    public Sprite Icon => _characterIcon;
    
    [Header("Base Stats (torso)")]
    [SerializeField] private int _defaultHP;
    [SerializeField] private int _defaultMaxHP;
    [SerializeField] private int _defaultAttack;
    [SerializeField] private int _defaultDefense;
    [SerializeField] private int _defaultHealAmount;
    [SerializeField] private int _defaultSpeed;

    [Header("Loadout (equipped parts / enemy drop pool)")]
    [SerializeField] private CreatureLoadout _loadout = new CreatureLoadout();
    public CreatureLoadout Loadout => _loadout;

    public ObservableValue<int> MaxHP { get; } = new ObservableValue<int>();
    public ObservableValue<int> HP { get; } = new ObservableValue<int>();
    public ObservableValue<int> Attack { get; } = new ObservableValue<int>();
    public ObservableValue<int> Defense { get; } = new ObservableValue<int>();
    public ObservableValue<int> HealAmount { get; } = new ObservableValue<int>();
    public ObservableValue<int> Speed { get; } = new ObservableValue<int>();

    public ObservableValue<int> Block { get; } = new ObservableValue<int>();
    public ObservableValue<int> WeaknessDuration { get; } = new ObservableValue<int>();
    public ObservableValue<int> VulnerabilityDuration { get; } = new ObservableValue<int>();
    public ObservableValue<int> Thorns { get; } = new ObservableValue<int>();

    public bool HasDied { get; private set; } = false;

    public event Action<int> OnCharacterTakeDamage;
    public event Action OnCharacterDeath;
    
    private void OnEnable()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        HasDied = false;
        MaxHP.Value = _defaultMaxHP;
        HP.Value = _defaultHP;
        Attack.Value = _defaultAttack;
        Defense.Value = _defaultDefense;
        HealAmount.Value = _defaultHealAmount;
        Speed.Value = _defaultSpeed;
        ResetCombatState();
        RecomputeStats();

        // *** DO NOT REMOVE — keeps body-part HP modifiers visible at spawn. ***
        // RecomputeStats raises MaxHP to include loadout modifiers, but doesn't touch HP.
        // Without this line a character authored at _defaultHP == _defaultMaxHP would start
        // at the BASE MaxHP value (e.g. 100/150) instead of the loadout-adjusted full HP (150/150).
        // The conditional preserves intent for characters intentionally authored below full HP.
        if (_defaultHP >= _defaultMaxHP)
        {
            HP.Value = MaxHP.Value;
        }
    }

    /// <summary>
    /// Zero per-encounter status values (Block, Weakness, Vulnerability, Thorns) without
    /// touching HP/MaxHP/Attack or other persistent stats. Called between encounters so
    /// statuses don't bleed from one fight into the next.
    /// </summary>
    public void ResetCombatState()
    {
        Block.Value = 0;
        WeaknessDuration.Value = 0;
        VulnerabilityDuration.Value = 0;
        Thorns.Value = 0;
    }

    public void RecomputeStats()
    {
        MaxHP.Value = _defaultMaxHP + _loadout.GetTotalMaxHPModifier();
        Attack.Value = _defaultAttack + _loadout.GetTotalAttackModifier();
        if (HP.Value > MaxHP.Value) HP.Value = MaxHP.Value;
    }

    /// <summary>
    /// Apply finalized damage to HP. Vulnerability and Block are already accounted for upstream
    /// by AttackService.DealDamage. This just decrements HP and fires events.
    /// </summary>
    public void ApplyDamage(int damageAmount)
    {
        if (damageAmount < 0) damageAmount = 0;
        HP.Value = Mathf.Max(0, HP.Value - damageAmount);

        if (damageAmount > 0)
        {
            OnCharacterTakeDamage?.Invoke(damageAmount);
        }

        if (HP.Value == 0 && !HasDied)
        {
            HasDied = true;
            OnCharacterDeath?.Invoke();
        }
    }

    public void HealToFull()
    {
        HP.Value = MaxHP.Value;
    }
}