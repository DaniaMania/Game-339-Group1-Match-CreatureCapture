using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Modal overlay that appears after the player selects a slot to swap into.
/// Shows old limb on one side, arrow, new limb on the other side, with name + info under each icon.
/// Confirm commits the swap; Back returns to slot-selection.
/// </summary>
public class UpgradeConfirmationUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup _panel;

    [Header("Old Limb")]
    [SerializeField] private Image _oldIcon;
    [SerializeField] private TextMeshProUGUI _oldNameLabel;
    [SerializeField] private TextMeshProUGUI _oldInfoLabel;

    [Header("New Limb")]
    [SerializeField] private Image _newIcon;
    [SerializeField] private TextMeshProUGUI _newNameLabel;
    [SerializeField] private TextMeshProUGUI _newInfoLabel;

    [Header("Buttons")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;

    public event Action OnConfirm;
    public event Action OnBack;

    private void Awake()
    {
        if (_confirmButton != null) _confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
        if (_backButton != null) _backButton.onClick.AddListener(() => OnBack?.Invoke());
        SetVisible(false);
    }

    public void Show(BodyPart oldPart, BodyPart newPart)
    {
        PopulateSide(_oldIcon, _oldNameLabel, _oldInfoLabel, oldPart);
        PopulateSide(_newIcon, _newNameLabel, _newInfoLabel, newPart);
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private static void PopulateSide(Image icon, TextMeshProUGUI nameLabel, TextMeshProUGUI infoLabel, BodyPart part)
    {
        if (part == null)
        {
            if (icon != null) icon.enabled = false;
            if (nameLabel != null) nameLabel.text = "(empty)";
            if (infoLabel != null) infoLabel.text = "";
            return;
        }

        if (icon != null)
        {
            bool hasIcon = part.icon != null;
            icon.enabled = hasIcon;
            if (hasIcon) icon.sprite = part.icon;
        }
        if (nameLabel != null) nameLabel.text = part.partName;
        if (infoLabel != null)
        {
            string subheading = BodyPartFormatter.FormatSubheading(part);
            string info = BodyPartFormatter.FormatPartInfo(part);
            infoLabel.text = string.IsNullOrEmpty(subheading) ? info : $"{subheading}\n{info}";
        }
    }

    private void SetVisible(bool visible)
    {
        if (_panel == null) return;
        _panel.alpha = visible ? 1f : 0f;
        _panel.interactable = visible;
        _panel.blocksRaycasts = visible;
    }
}
