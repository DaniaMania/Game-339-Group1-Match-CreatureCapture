using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Shared tooltip panel. One instance lives in the Canvas and is moved/populated
// on demand by any UI element that wants to display ability info on hover.
public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("References")]
    [SerializeField] private RectTransform _panel;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _effectLabel;
    [SerializeField] private TextMeshProUGUI _cooldownLabel;
    [SerializeField] private TextMeshProUGUI _descriptionLabel;

    [Header("Positioning")]
    [Tooltip("Pixel offset from the anchor's pivot. Set panel pivot to (0.5, 0) to make the tooltip appear above the anchor.")]
    [SerializeField] private Vector2 _offset = new Vector2(0f, 80f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        Hide();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Populate and show the tooltip. cooldownInfo and icon are optional —
    /// pass null/empty to hide their respective UI elements.
    /// </summary>
    public void Show(string name, string effect, string description, RectTransform anchor, string cooldownInfo = null, Sprite icon = null)
    {
        if (anchor == null) return;

        if (_nameLabel != null) _nameLabel.text = name ?? "";
        if (_effectLabel != null) _effectLabel.text = effect ?? "";
        if (_descriptionLabel != null) _descriptionLabel.text = description ?? "";

        if (_cooldownLabel != null)
        {
            bool hasCooldown = !string.IsNullOrEmpty(cooldownInfo);
            _cooldownLabel.gameObject.SetActive(hasCooldown);
            if (hasCooldown) _cooldownLabel.text = cooldownInfo;
        }

        if (_iconImage != null)
        {
            bool hasIcon = icon != null;
            _iconImage.gameObject.SetActive(hasIcon);
            if (hasIcon) _iconImage.sprite = icon;
        }

        _panel.position = anchor.position + (Vector3)_offset;
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = visible ? 1f : 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}