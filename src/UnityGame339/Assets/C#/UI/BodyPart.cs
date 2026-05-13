using UnityEngine;

// ScriptableObject representing a single body part.
// Create via: Assets > Create > Creature > Body Part

[CreateAssetMenu(fileName = "New BodyPart", menuName = "Creature/Body Part")]
public class BodyPart : ScriptableObject
{
    [Header("Identity")]
    public string partName = "Unnamed Part";
    public BodyPartType partType;
    public Sprite icon;

    [Header("Stat Modifiers (always applied while equipped, can be negative)")]
    public int maxHPModifier = 0;
    public int attackModifier = 0;

    [Header("Active Ability (arms — used on player's turn)")]
    public string abilityName = "Ability";
    [TextArea] public string abilityDescription = "Deals damage.";

    [Tooltip("What this ability does. Attack uses the owner's Attack stat per hit; other types use abilityValue per hit.")]
    public AbilityType abilityType = AbilityType.Attack;

    [Tooltip("Per-hit value. Attack: ignored (uses Attack stat). Heal: HP per hit. Shield: block per hit. Weakness/Vulnerability: duration per hit (total duration = value * hits).")]
    public int abilityValue = 0;

    [Tooltip("How many times this ability fires. Multihit deals damage / heals / shields per hit; debuffs stack duration per hit.")]
    public int abilityHits = 1;

    [Tooltip("After use, this ability is locked for this many of the owner's turns. 0 = no cooldown.")]
    public int cooldownTurns = 0;

    [Header("Passive Effect (legs — triggers each turn)")]
    [TextArea] public string passiveDescription = "No passive.";
    public PassiveType passiveType = PassiveType.None;
    public int passiveValue = 0;
}

public enum BodyPartType
{
    Arm,
    Leg
}

public enum AbilityType
{
    Attack,
    Heal,
    Shield,
    Weakness,
    Vulnerability
}

public enum PassiveType
{
    None,
    Block,
    Regen,
    Thorns
}