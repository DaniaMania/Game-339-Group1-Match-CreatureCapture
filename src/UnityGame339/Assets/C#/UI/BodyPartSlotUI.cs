using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BodyPartSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image slotIcon;
    [SerializeField] private TextMeshProUGUI slotLabel;
    [SerializeField] private GameObject emptyOverlay;
    [SerializeField] private Button button;

    [Header("Config")]
    public BodyPartType slotType;

    // Fired when the player clicks this slot
    // Parameter is the slot's type so the parent UI knows which slot was tapped
    public event Action<BodyPartType> OnSlotClicked;

    private BodyPart _equippedPart;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(() => OnSlotClicked?.Invoke(slotType));
    }

    // Call this to refresh the slot's display
    public void Populate(BodyPart part)
    {
        _equippedPart = part;

        bool haspart = part != null;
        if (emptyOverlay != null) emptyOverlay.SetActive(!haspart);

        if (slotLabel != null)
            slotLabel.text = haspart ? part.partName : slotType.ToString();

        if (slotIcon != null)
        {
            slotIcon.enabled = haspart && part.icon != null;
            if (haspart && part.icon != null)
                slotIcon.sprite = part.icon;
        }
    }

    public void SetInteractable(bool interactable) => button.interactable = interactable;

    public BodyPart GetEquippedPart() => _equippedPart;
}
