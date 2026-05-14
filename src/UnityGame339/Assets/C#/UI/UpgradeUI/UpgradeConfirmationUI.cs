using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Modal overlay shown after the player picks a slot to swap into.
/// Per side: icon, part name, skill name (ability for arms / passive for legs),
/// computed effect, cooldown (if applicable), description + stat mods.
/// Mirrors the in-battle tooltip's text structure.
/// Confirm commits the swap; Back returns to slot-selection.
/// </summary>
public class UpgradeConfirmationUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup _panel;

    [Header("Old Limb")]
    [SerializeField] private Image _oldIcon;
    [SerializeField] private TextMeshProUGUI _oldNameLabel;
    [SerializeField] private TextMeshProUGUI _oldSkillNameLabel;
    [SerializeField] private TextMeshProUGUI _oldEffectLabel;
    [SerializeField] private TextMeshProUGUI _oldCooldownLabel;
    [SerializeField] private TextMeshProUGUI _oldDescriptionLabel;

    [Header("New Limb")]
    [SerializeField] private Image _newIcon;
    [SerializeField] private TextMeshProUGUI _newNameLabel;
    [SerializeField] private TextMeshProUGUI _newSkillNameLabel;
    [SerializeField] private TextMeshProUGUI _newEffectLabel;
    [SerializeField] private TextMeshProUGUI _newCooldownLabel;
    [SerializeField] private TextMeshProUGUI _newDescriptionLabel;

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

    /// <summary>
    /// owner is the character whose stats are used to compute attack-scaling effect text (typically the player).
    /// </summary>
    public void Show(BodyPart oldPart, BodyPart newPart, Character owner)
    {
        PopulateSide(_oldIcon, _oldNameLabel, _oldSkillNameLabel, _oldEffectLabel, _oldCooldownLabel, _oldDescriptionLabel, oldPart, owner);
        PopulateSide(_newIcon, _newNameLabel, _newSkillNameLabel, _newEffectLabel, _newCooldownLabel, _newDescriptionLabel, newPart, owner);
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private static void PopulateSide(
        Image icon,
        TextMeshProUGUI nameLabel,
        TextMeshProUGUI skillNameLabel,
        TextMeshProUGUI effectLabel,
        TextMeshProUGUI cooldownLabel,
        TextMeshProUGUI descriptionLabel,
        BodyPart part,
        Character owner)
    {
        if (part == null)
        {
            if (icon != null) icon.enabled = false;
            if (nameLabel != null) nameLabel.text = "(empty)";
            if (skillNameLabel != null) skillNameLabel.text = "";
            if (effectLabel != null) effectLabel.text = "";
            if (cooldownLabel != null) cooldownLabel.gameObject.SetActive(false);
            if (descriptionLabel != null) descriptionLabel.text = "";
            return;
        }

        if (icon != null)
        {
            bool hasIcon = part.icon != null;
            icon.enabled = hasIcon;
            if (hasIcon) icon.sprite = part.icon;
        }

        if (nameLabel != null) nameLabel.text = part.partName;
        if (skillNameLabel != null) skillNameLabel.text = BodyPartFormatter.FormatSubheading(part);
        if (effectLabel != null) effectLabel.text = BodyPartFormatter.FormatEffect(part, owner);

        if (cooldownLabel != null)
        {
            string cooldownText = BodyPartFormatter.FormatCooldown(part);
            bool hasCooldown = !string.IsNullOrEmpty(cooldownText);
            cooldownLabel.gameObject.SetActive(hasCooldown);
            if (hasCooldown) cooldownLabel.text = cooldownText;
        }

        if (descriptionLabel != null) descriptionLabel.text = BodyPartFormatter.FormatPartInfo(part);
    }

    private void SetVisible(bool visible)
    {
        if (_panel == null) return;
        _panel.alpha = visible ? 1f : 0f;
        _panel.interactable = visible;
        _panel.blocksRaycasts = visible;
    }
}