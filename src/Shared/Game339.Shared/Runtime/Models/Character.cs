using System;
using Game339.Shared;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private Sprite _characterIcon;
    public Sprite Icon => _characterIcon;
    
    [SerializeField] private int _defaultHP;
    [SerializeField] private int _defaultMaxHP;
    [SerializeField] private int _defaultAttack;
    [SerializeField] private int _defaultDefense;
    [SerializeField] private int _defaultHealAmount;
    [SerializeField] private int _defaultSpeed;
    
    public ObservableValue<int> MaxHP { get; } = new ObservableValue<int>();
    public ObservableValue<int> HP { get; } = new ObservableValue<int>();
    public ObservableValue<int> Attack { get; } = new ObservableValue<int>();
    public ObservableValue<int> Defense { get; } = new ObservableValue<int>();
    public ObservableValue<int> HealAmount { get; } = new ObservableValue<int>();
    public ObservableValue<int> Speed { get; } = new ObservableValue<int>();

    public event Action<int> OnCharacterTakeDamage;
    public event Action OnCharacterDeath;
    
    private void OnEnable()
    {
        MaxHP.Value = _defaultMaxHP;
        HP.Value = _defaultHP;
        Attack.Value = _defaultAttack;
        Defense.Value = _defaultDefense;
        HealAmount.Value = _defaultHealAmount;
        Speed.Value = _defaultSpeed;
    }

    public void ResetValues()
    {
        MaxHP.Value = _defaultMaxHP;
        HP.Value = _defaultHP;
        Attack.Value = _defaultAttack;
        Defense.Value = _defaultDefense;
        HealAmount.Value = _defaultHealAmount;
        Speed.Value = _defaultSpeed;
    }

    public void ApplyDamage(int damageAmount)
    {
        int remainingHealth = HP.Value - damageAmount;
        HP.Value = Mathf.Max(0, remainingHealth);
        
        if (HP.Value == 0) OnCharacterDeath?.Invoke();
        else OnCharacterTakeDamage?.Invoke(damageAmount);
    }
    
    public void HealToFull()
    {
        HP.Value = MaxHP.Value;
    }
}