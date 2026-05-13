using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A right-side offered limb on the upgrade screen. Shows icon + part name.
/// Hover triggers the shared tooltip (no icon, per design).
/// Click fires OnClicked. Selected-state visuals are handled by Unity's built-in Button transition.
/// </summary>
public class OfferedLimbUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameLabel;

    public event Action<OfferedLimbUI> OnClicked;
    public event Action<OfferedLimbUI> OnHoverEnter;
    public event Action<OfferedLimbUI> OnHoverExit;

    public BodyPart Part { get; private set; }

    private void Awake()
    {
        if (_button == null) _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnClicked?.Invoke(this));
    }

    public void Populate(BodyPart part)
    {
        Part = part;
        gameObject.SetActive(part != null);
        if (part == null) return;

        if (_iconImage != null)
        {
            _iconImage.enabled = part.icon != null;
            if (part.icon != null) _iconImage.sprite = part.icon;
        }
        if (_nameLabel != null) _nameLabel.text = part.partName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
        OnHoverEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null) TooltipUI.Instance.Hide();
        OnHoverExit?.Invoke(this);
    }

    private void ShowTooltip()
    {
        if (Part == null || TooltipUI.Instance == null) return;

        string title = Part.partName;
        string subheading = BodyPartFormatter.FormatSubheading(Part);
        string description = BodyPartFormatter.FormatPartInfo(Part);

        TooltipUI.Instance.Show(title, subheading, description, (RectTransform)transform,
            cooldownInfo: null, icon: null);
    }
}