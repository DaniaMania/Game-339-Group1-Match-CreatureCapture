using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// One limb in the side-panel player preview. Just an icon + hover-for-tooltip.
/// Configured per-slot in the inspector with its BodyPartType + slot index, the same way
/// CreatureLimbSlotUI is configured for the upgrade screen.
/// </summary>
public class HoverableLimbUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;

    [Header("Slot Identity (set per slot in inspector)")]
    [SerializeField] private BodyPartType _slotType = BodyPartType.Arm;
    [SerializeField] private int _slotIndex = 0;

    public BodyPartType SlotType => _slotType;
    public int SlotIndex => _slotIndex;

    private BodyPart _part;
    private Character _owner;

    public void Populate(BodyPart part, Character owner)
    {
        _part = part;
        _owner = owner;

        if (_iconImage != null)
        {
            bool hasIcon = part != null && part.icon != null;
            // Disable the Image rather than the whole GameObject so layout stays consistent
            // and raycasting stops automatically — hover does nothing when there's no part.
            _iconImage.enabled = hasIcon;
            if (hasIcon) _iconImage.sprite = part.icon;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_part == null || TooltipUI.Instance == null) return;

        string title = _part.partName;
        string effect = BodyPartFormatter.FormatEffect(_part, _owner);
        string cooldown = BodyPartFormatter.FormatCooldown(_part);
        string description = BodyPartFormatter.FormatPartInfo(_part);

        TooltipUI.Instance.Show(title, effect, description, (RectTransform)transform, cooldown, _part.icon);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null) TooltipUI.Instance.Hide();
    }
}
