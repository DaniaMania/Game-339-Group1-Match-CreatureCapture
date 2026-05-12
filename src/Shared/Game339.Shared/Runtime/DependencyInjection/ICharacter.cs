using System;

namespace Game339.Shared.DependencyInjection
{
    public interface ICharacter
    {
        public ObservableValue<int> MaxHP { get; }
        public ObservableValue<int> HP { get; }
        public ObservableValue<int> Attack { get; }
        public ObservableValue<int> Defense { get; }
        public ObservableValue<int> HealAmount { get; }
        public ObservableValue<int> Speed { get; }

        public ObservableValue<int> Block { get; }
        public ObservableValue<int> WeaknessDuration { get; }
        public ObservableValue<int> VulnerabilityDuration { get; }

        public bool HasDied { get; }

        public event Action<int> OnCharacterTakeDamage;
        public event Action OnCharacterDeath;

        public void ApplyDamage(int damageAmount);
    }
}