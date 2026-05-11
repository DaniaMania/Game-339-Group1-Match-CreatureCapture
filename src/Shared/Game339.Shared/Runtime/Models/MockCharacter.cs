using System;
using Game339.Shared.DependencyInjection;

namespace Game339.Shared.Models
{
    public class MockCharacter: ICharacter
    {
        public ObservableValue<int> MaxHP { get; } = new ObservableValue<int>();
        public ObservableValue<int> HP { get; } = new ObservableValue<int>();
        public ObservableValue<int> Attack { get; } = new ObservableValue<int>();
        public ObservableValue<int> Defense { get; } = new ObservableValue<int>();
        public ObservableValue<int> HealAmount { get; } = new ObservableValue<int>();
        public ObservableValue<int> Speed { get; } = new ObservableValue<int>();

        public ObservableValue<int> Block { get; } = new ObservableValue<int>();
        public ObservableValue<int> WeaknessDuration { get; } = new ObservableValue<int>();
        public ObservableValue<int> VulnerabilityDuration { get; } = new ObservableValue<int>();
        
        public bool HasDied { get; private set; }
        
        public event Action<int> OnCharacterTakeDamage;
        public event Action OnCharacterDeath;
        
        public void ApplyDamage(int damageAmount)
        {
            int remainingHealth = HP.Value - damageAmount;
            HP.Value = Math.Max(0, remainingHealth);

            if (HP.Value == 0)
            {
                HasDied = true;
                OnCharacterDeath?.Invoke();
            }
            else OnCharacterTakeDamage?.Invoke(damageAmount);
        }
    }
}