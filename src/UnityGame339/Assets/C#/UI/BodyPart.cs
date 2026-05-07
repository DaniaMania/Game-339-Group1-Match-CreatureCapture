using UnityEngine;


// ScriptableObject representing a single body part
// Create via: Assets > Create > Creature > Body Part

[CreateAssetMenu(fileName = "New BodyPart", menuName = "Creature/Body Part")]
public class BodyPart : ScriptableObject
{
    [Header("Identity")]
    public string partName = "Unnamed Part";
    public BodyPartType partType;
    public Sprite icon;

    [Header("Active Ability (used on player's turn)")]
    public string abilityName = "Attack";
    [TextArea] public string abilityDescription = "Deals damage.";
    public int abilityBaseDamage = 10;

    [Header("Passive Effect (triggers each turn)")]
    [TextArea] public string passiveDescription = "No passive.";
    public int passiveValue = 0;

    // TODO: Add status effects (stun, poison, shield, etc.)
    // TODO: Add ability cooldown/cost fields when resource system is ready
    // TODO: Add rarity tier for the merge/capture system
}

public enum BodyPartType
{
    Head,
    LeftArm,
    Torso,
    RightArm,
    LeftLeg,
    RightLeg
}