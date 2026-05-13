using System.Collections.Generic;

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
    /// Suitable for the description area of a tooltip or the info panel of the confirmation modal.
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

    private static string FormatStat(int value, string statName)
    {
        string sign = value > 0 ? "+" : "";
        return $"{sign}{value} {statName}";
    }
}
