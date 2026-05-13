using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Center area on the upgrade screen showing the player creature with all 4 swappable limbs.
/// Head is decorative (uses the player's character icon). Torso is decorative.
/// Limb slots are clickable when their type matches the currently selected offer.
/// </summary>
public class CreaturePreviewUI : MonoBehaviour
{
    [Header("Decorative (non-interactable)")]
    [SerializeField] private Image _headIcon;
    [SerializeField] private GameObject _torsoVisual;

    [Header("Limb Slots — assign 2 arm and 2 leg slot UIs in any order")]
    [SerializeField] private CreatureLimbSlotUI[] _limbSlots;

    /// <summary>
    /// Fires when any limb slot is clicked. (BodyPartType, slotIndex) identify which one.
    /// </summary>
    public event Action<BodyPartType, int> OnSlotClicked;

    private void Awake()
    {
        if (_limbSlots == null) return;
        foreach (CreatureLimbSlotUI slot in _limbSlots)
        {
            if (slot == null) continue;
            slot.OnClicked += HandleSlotClicked;
        }
    }

    private void OnDestroy()
    {
        if (_limbSlots == null) return;
        foreach (CreatureLimbSlotUI slot in _limbSlots)
        {
            if (slot == null) continue;
            slot.OnClicked -= HandleSlotClicked;
        }
    }

    public void Populate(Character player)
    {
        if (_headIcon != null)
        {
            _headIcon.enabled = player != null && player.Icon != null;
            if (player != null && player.Icon != null) _headIcon.sprite = player.Icon;
        }

        if (_limbSlots == null) return;
        BodyPart[] arms = player?.Loadout?.arms;
        BodyPart[] legs = player?.Loadout?.legs;

        foreach (CreatureLimbSlotUI slot in _limbSlots)
        {
            if (slot == null) continue;
            BodyPart equipped = GetEquippedForSlot(slot, arms, legs);
            slot.Populate(equipped);
            slot.SetInteractable(false);
        }
    }

    /// <summary>
    /// Enable/disable all slots matching the given type. Used by UpgradeController to gate
    /// which slots can be clicked based on the currently selected offer's type.
    /// </summary>
    public void SetSlotsInteractable(BodyPartType type, bool interactable)
    {
        if (_limbSlots == null) return;
        foreach (CreatureLimbSlotUI slot in _limbSlots)
        {
            if (slot == null) continue;
            if (slot.SlotType == type) slot.SetInteractable(interactable);
        }
    }

    public void DisableAllSlots()
    {
        if (_limbSlots == null) return;
        foreach (CreatureLimbSlotUI slot in _limbSlots)
        {
            if (slot == null) continue;
            slot.SetInteractable(false);
        }
    }

    private static BodyPart GetEquippedForSlot(CreatureLimbSlotUI slot, BodyPart[] arms, BodyPart[] legs)
    {
        BodyPart[] source = (slot.SlotType == BodyPartType.Arm) ? arms : legs;
        if (source == null || slot.SlotIndex < 0 || slot.SlotIndex >= source.Length) return null;
        return source[slot.SlotIndex];
    }

    private void HandleSlotClicked(CreatureLimbSlotUI slot)
    {
        OnSlotClicked?.Invoke(slot.SlotType, slot.SlotIndex);
    }
}
