using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single clickable limb slot in the creature preview at the center of the upgrade screen.
/// Shows the equipped part's icon; click fires OnClicked when interactable.
/// Configured per-slot in the inspector with its BodyPartType + slot index.
/// </summary>
public class CreatureLimbSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;
    [Tooltip("Optional GameObject (e.g. glow/outline) toggled on when this slot is interactable.")]
    [SerializeField] private GameObject _interactableIndicator;

    [Header("Slot Identity (set per slot in inspector)")]
    [SerializeField] private BodyPartType _slotType = BodyPartType.Arm;
    [SerializeField] private int _slotIndex = 0;

    public BodyPartType SlotType => _slotType;
    public int SlotIndex => _slotIndex;

    public event Action<CreatureLimbSlotUI> OnClicked;

    private BodyPart _currentPart;

    private void Awake()
    {
        if (_button == null) _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnClicked?.Invoke(this));
    }

    public void Populate(BodyPart part)
    {
        _currentPart = part;
        if (_iconImage != null)
        {
            bool hasIcon = part != null && part.icon != null;
            _iconImage.enabled = hasIcon;
            if (hasIcon) _iconImage.sprite = part.icon;
        }
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
        if (_interactableIndicator != null) _interactableIndicator.SetActive(interactable);
    }

    public BodyPart GetCurrentPart() => _currentPart;
}
