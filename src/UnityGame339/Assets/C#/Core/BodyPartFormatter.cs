using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared text formatting for BodyPart info. Used by tooltips, the confirmation modal, etc.
/// </summary>
public static class BodyPartFormatter
{
    /// <summary>
    /// Returns e.g. "+5 HP, -2 ATK". Empty string when the part has no stat modifiers.
    /// </summary>
    public static string FormatStatModifiers(BodyPart part)
    {
        if (part == null) return "";
        List<string> bits = new List<string>();
        if (part.maxHPModifier != 0) bits.Add(FormatStat(part.maxHPModifier, "HP"));
        if (part.attackModifier != 0) bits.Add(FormatStat(part.attackModifier, "ATK"));
        return string.Join(", ", bits);
    }

    /// <summary>
    /// Returns a multi-line block describing the part: ability/passive description + stat modifiers.
    /// </summary>
    public static string FormatPartInfo(BodyPart part)
    {
        if (part == null) return "";
        string description = part.partType == BodyPartType.Arm
            ? part.abilityDescription
            : part.passiveDescription;

        string mods = FormatStatModifiers(part);
        if (string.IsNullOrEmpty(mods)) return description ?? "";
        if (string.IsNullOrEmpty(description)) return mods;
        return description + "\n" + mods;
    }

    /// <summary>
    /// Short subheading shown above the description — ability name for arms, passive type for legs.
    /// </summary>
    public static string FormatSubheading(BodyPart part)
    {
        if (part == null) return "";
        if (part.partType == BodyPartType.Arm) return part.abilityName ?? "";
        return part.passiveType != PassiveType.None ? part.passiveType.ToString() : "";
    }

    /// <summary>
    /// Computed gameplay effect string, e.g. "Deal 30 damage" or "Apply Vulnerability for 5 turn(s) (+50% damage taken)".
    /// For arm Attack abilities, scales with owner.Attack.Value (uses 0 if owner is null).
    /// For legs, describes the passive based on its type and value.
    /// </summary>
    public static string FormatEffect(BodyPart part, Character owner)
    {
        if (part == null) return "";
        if (part.partType == BodyPartType.Arm) return FormatActiveAbilityEffect(part, owner);
        if (part.partType == BodyPartType.Leg) return FormatPassiveEffect(part);
        return "";
    }

    /// <summary>
    /// Returns null when the part has no cooldown (callers should hide their cooldown label on null).
    /// currentCooldown > 0 adds a "(N remaining)" suffix — leave 0 outside an active battle.
    /// </summary>
    public static string FormatCooldown(BodyPart part, int currentCooldown = 0)
    {
        if (part == null || part.partType != BodyPartType.Arm) return null;
        if (part.cooldownTurns <= 0) return null;
        if (currentCooldown > 0)
        {
            return $"Cooldown: {part.cooldownTurns} turns ({currentCooldown} remaining)";
        }
        return $"Cooldown: {part.cooldownTurns} turns";
    }

    //===== Internals =====

    private static string FormatActiveAbilityEffect(BodyPart part, Character owner)
    {
        int hits = Mathf.Max(1, part.abilityHits);

        switch (part.abilityType)
        {
            case AbilityType.Attack:
            {
                int perHit = owner != null ? owner.Attack.Value : 0;
                int total = perHit * hits;
                return hits > 1
                    ? $"Deal {perHit} damage x {hits} hits ({total} total)"
                    : $"Deal {perHit} damage";
            }
            case AbilityType.Heal:
            {
                int perHit = part.abilityValue;
                int total = perHit * hits;
                return hits > 1
                    ? $"Heal {perHit} HP x {hits} ({total} total)"
                    : $"Heal {perHit} HP";
            }
            case AbilityType.Shield:
            {
                int perHit = part.abilityValue;
                int total = perHit * hits;
                return hits > 1
                    ? $"Gain {perHit} block x {hits} ({total} total)"
                    : $"Gain {perHit} block";
            }
            case AbilityType.Weakness:
            {
                int totalDuration = part.abilityValue * hits;
                return $"Apply Weakness for {totalDuration} turn(s) (-25% target attack)";
            }
            case AbilityType.Vulnerability:
            {
                int totalDuration = part.abilityValue * hits;
                return $"Apply Vulnerability for {totalDuration} turn(s) (+50% damage taken)";
            }
            default:
                return "";
        }
    }

    private static string FormatPassiveEffect(BodyPart part)
    {
        switch (part.passiveType)
        {
            case PassiveType.Block:
                return $"Gain {part.passiveValue} Block at the start of each turn";
            case PassiveType.Regen:
                return $"Heal {part.passiveValue} HP at the start of each turn";
            case PassiveType.Thorns:
                return $"Deal {part.passiveValue} damage to attackers";
            case PassiveType.None:
            default:
                return "";
        }
    }

    private static string FormatStat(int value, string statName)
    {
        string sign = value > 0 ? "+" : "";
        return $"{sign}{value} {statName}";
    }
}