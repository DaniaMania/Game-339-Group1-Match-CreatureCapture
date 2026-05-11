namespace Game339.Shared.DependencyInjection
{
    /// <summary>
    /// Extension methods for ICharacter combat helpers. Living in shared keeps them
    /// testable without Unity. Consumers need: using Game339.Shared.DependencyInjection;
    /// </summary>
    public static class CharacterCombatExtensions
    {
        /// <summary>
        /// Heal up to MaxHP. Negative or zero amounts are no-ops.
        /// </summary>
        public static void Heal(this ICharacter character, int amount)
        {
            if (amount <= 0) return;
            int healed = character.HP.Value + amount;
            if (healed > character.MaxHP.Value) healed = character.MaxHP.Value;
            character.HP.Value = healed;
        }

        /// <summary>
        /// Add block. Block absorbs incoming damage before HP and persists across turns until consumed.
        /// </summary>
        public static void AddBlock(this ICharacter character, int amount)
        {
            if (amount <= 0) return;
            character.Block.Value += amount;
        }

        /// <summary>
        /// Apply or extend Weakness. While Weakness lasts, this character's outgoing attacks deal -25% damage.
        /// </summary>
        public static void ApplyWeakness(this ICharacter character, int turns)
        {
            if (turns <= 0) return;
            character.WeaknessDuration.Value += turns;
        }

        /// <summary>
        /// Apply or extend Vulnerability. While Vulnerable, this character takes +50% damage from all sources.
        /// </summary>
        public static void ApplyVulnerability(this ICharacter character, int turns)
        {
            if (turns <= 0) return;
            character.VulnerabilityDuration.Value += turns;
        }

        /// <summary>
        /// Decrement status durations by 1 (clamped at 0). Call at the end of this character's own turn.
        /// </summary>
        public static void TickStatuses(this ICharacter character)
        {
            if (character.WeaknessDuration.Value > 0) character.WeaknessDuration.Value--;
            if (character.VulnerabilityDuration.Value > 0) character.VulnerabilityDuration.Value--;
        }
    }
}
