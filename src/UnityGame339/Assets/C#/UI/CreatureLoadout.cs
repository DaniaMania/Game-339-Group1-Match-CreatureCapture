using UnityEngine;
using System.Collections.Generic;

// Holds the equipped body parts for a creature.
// Embedded as a serialized field on Character. Character holds base stats;
// this just tracks which parts are equipped and aggregates their modifiers.

[System.Serializable]
public class CreatureLoadout
{
    [Header("Swappable Slots")]
    public BodyPart[] arms = new BodyPart[2];
    public BodyPart[] legs = new BodyPart[2];

    // Yields every equipped part (non-null only).
    public IEnumerable<BodyPart> GetEquippedParts()
    {
        if (arms != null)
        {
            foreach (BodyPart arm in arms)
                if (arm != null) yield return arm;
        }
        if (legs != null)
        {
            foreach (BodyPart leg in legs)
                if (leg != null) yield return leg;
        }
    }

    public int GetTotalMaxHPModifier()
    {
        int total = 0;
        foreach (BodyPart part in GetEquippedParts()) total += part.maxHPModifier;
        return total;
    }

    public int GetTotalAttackModifier()
    {
        int total = 0;
        foreach (BodyPart part in GetEquippedParts()) total += part.attackModifier;
        return total;
    }
}