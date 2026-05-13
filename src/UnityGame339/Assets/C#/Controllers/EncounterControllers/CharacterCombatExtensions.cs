namespace Game339.Shared.DependencyInjection
{
    /// <summary>
    /// Extension methods for ICharacter combat helpers. Living in shared keeps them
    /// testable without Unity. Consumers need: using Game339.Shared.DependencyInjection;
    /// </summary>
    public static class CharacterCombatExtensions
    {
        public static void Heal(this ICharacter character, int amount)
        {
            if (amount <= 0) return;
            int healed = character.HP.Value + amount;
            if (healed > character.MaxHP.Value) healed = character.MaxHP.Value;
            character.HP.Value = healed;
        }

        public static void AddBlock(this ICharacter character, int amount)
        {
            if (amount <= 0) return;
            character.Block.Value += amount;
        }

        public static void ApplyWeakness(this ICharacter character, int turns)
        {
            if (turns <= 0) return;
            character.WeaknessDuration.Value += turns;
        }

        public static void ApplyVulnerability(this ICharacter character, int turns)
        {
            if (turns <= 0) return;
            character.VulnerabilityDuration.Value += turns;
        }

        /// <summary>
        /// Add thorns. Persists across turns (not consumed by hits), decays by 1 at end of own turn.
        /// </summary>
        public static void ApplyThorns(this ICharacter character, int amount)
        {
            if (amount <= 0) return;
            character.Thorns.Value += amount;
        }

        /// <summary>
        /// Decrement each timed status by 1 (clamped at 0). Call at end of this character's own turn.
        /// Affects Weakness, Vulnerability, and Thorns.
        /// </summary>
        public static void TickStatuses(this ICharacter character)
        {
            if (character.WeaknessDuration.Value > 0) character.WeaknessDuration.Value--;
            if (character.VulnerabilityDuration.Value > 0) character.VulnerabilityDuration.Value--;
            if (character.Thorns.Value > 0) character.Thorns.Value--;
        }
    }
}