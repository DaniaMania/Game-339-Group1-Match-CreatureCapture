using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player creature preview in the side panel. Same visual structure as the upgrade-screen
/// CreaturePreviewUI but limbs are hoverable-only (not clickable for slot selection).
/// </summary>
public class PlayerPreviewUI : MonoBehaviour
{
    [Header("Decorative (non-interactable)")]
    [SerializeField] private Image _headIcon;
    [SerializeField] private GameObject _torsoVisual;

    [Header("Limb Slots — assign 2 arm and 2 leg slot UIs in any order")]
    [SerializeField] private HoverableLimbUI[] _limbSlots;

    private Character _player;

    public void Initialize(Character player)
    {
        _player = player;

        if (_headIcon != null)
        {
            bool hasIcon = player != null && player.Icon != null;
            _headIcon.enabled = hasIcon;
            if (hasIcon) _headIcon.sprite = player.Icon;
        }

        RefreshLimbs();
    }

    public void Deinitialize()
    {
        _player = null;
    }

    /// <summary>
    /// Re-read the player's current loadout and update all limb displays.
    /// Call after the loadout has changed (e.g. post-upgrade) or at the start of each encounter.
    /// </summary>
    public void RefreshLimbs()
    {
        if (_player == null || _limbSlots == null) return;
        BodyPart[] arms = _player.Loadout?.arms;
        BodyPart[] legs = _player.Loadout?.legs;

        foreach (HoverableLimbUI slot in _limbSlots)
        {
            if (slot == null) continue;
            BodyPart equipped = GetEquippedForSlot(slot, arms, legs);
            slot.Populate(equipped, _player);
        }
    }

    private static BodyPart GetEquippedForSlot(HoverableLimbUI slot, BodyPart[] arms, BodyPart[] legs)
    {
        BodyPart[] source = (slot.SlotType == BodyPartType.Arm) ? arms : legs;
        if (source == null || slot.SlotIndex < 0 || slot.SlotIndex >= source.Length) return null;
        return source[slot.SlotIndex];
    }
}
