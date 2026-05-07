using UnityEngine;
using System.Collections.Generic;

// Holds the six body part slots for a creature
// Attach to a GameObject or embed in a MonoBehaviour as a serialized field
[System.Serializable]
public class CreatureLoadout
{
    [Header("Creature Identity")]
    public string creatureName = "My Creature";
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Body Parts")]
    public BodyPart head;
    public BodyPart leftArm;
    public BodyPart torso;
    public BodyPart rightArm;
    public BodyPart leftLeg;
    public BodyPart rightLeg;

    // Accessors

    public BodyPart GetPart(BodyPartType type)
    {
        return type switch
        {
            BodyPartType.Head      => head,
            BodyPartType.LeftArm   => leftArm,
            BodyPartType.Torso     => torso,
            BodyPartType.RightArm  => rightArm,
            BodyPartType.LeftLeg   => leftLeg,
            BodyPartType.RightLeg  => rightLeg,
            _                      => null
        };
    }

    public void SetPart(BodyPartType type, BodyPart part)
    {
        switch (type)
        {
            case BodyPartType.Head:      head      = part; break;
            case BodyPartType.LeftArm:   leftArm   = part; break;
            case BodyPartType.Torso:     torso     = part; break;
            case BodyPartType.RightArm:  rightArm  = part; break;
            case BodyPartType.LeftLeg:   leftLeg   = part; break;
            case BodyPartType.RightLeg:  rightLeg  = part; break;
        }
    }

    // Returns all equipped parts (non-null only)
    public List<BodyPart> GetEquippedParts()
    {
        var parts = new List<BodyPart>();
        foreach (BodyPartType t in System.Enum.GetValues(typeof(BodyPartType)))
        {
            var p = GetPart(t);
            if (p != null) parts.Add(p);
        }
        return parts;
    }

    // Returns all equipped parts that have a valid active ability
    public List<BodyPart> GetActiveAbilities() => GetEquippedParts();

    public bool IsAlive => currentHealth > 0;

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    // TODO: Add status effect list (poisoned, stunned, shielded…)
    // TODO: Add merge logic when two same-creature loadouts are combined
}