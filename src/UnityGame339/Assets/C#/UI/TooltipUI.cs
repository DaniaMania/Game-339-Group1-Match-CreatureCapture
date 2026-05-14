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
    [Tooltip("Minimum padding kept between the tooltip and the canvas edges when clamping.")]
    [SerializeField] private float _edgePadding = 8f;

    private Canvas _cachedCanvas;

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
        _panel.SetAsLastSibling();
        SetVisible(true);

        // Force a layout rebuild so a content-sized panel reports its actual size before
        // we measure it. Without this, GetWorldCorners returns last frame's dimensions and
        // the clamp uses stale numbers.
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);

        ClampToCanvasBounds();
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

    /// <summary>
    /// Nudge the panel so it stays inside the root canvas's rect, with _edgePadding of slack.
    /// Works for any canvas render mode since both canvas and panel corners come from the same space.
    /// </summary>
    private void ClampToCanvasBounds()
    {
        if (_cachedCanvas == null) _cachedCanvas = _panel.GetComponentInParent<Canvas>();
        if (_cachedCanvas == null) return;
        RectTransform canvasRect = _cachedCanvas.transform as RectTransform;
        if (canvasRect == null) return;

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        // Corner order: 0 = bottom-left, 1 = top-left, 2 = top-right, 3 = bottom-right.

        Vector3[] selfCorners = new Vector3[4];
        _panel.GetWorldCorners(selfCorners);

        float leftEdge = canvasCorners[0].x + _edgePadding;
        float rightEdge = canvasCorners[2].x - _edgePadding;
        float bottomEdge = canvasCorners[0].y + _edgePadding;
        float topEdge = canvasCorners[2].y - _edgePadding;

        Vector3 adjust = Vector3.zero;
        if (selfCorners[2].x > rightEdge) adjust.x = rightEdge - selfCorners[2].x;
        else if (selfCorners[0].x < leftEdge) adjust.x = leftEdge - selfCorners[0].x;

        if (selfCorners[2].y > topEdge) adjust.y = topEdge - selfCorners[2].y;
        else if (selfCorners[0].y < bottomEdge) adjust.y = bottomEdge - selfCorners[0].y;

        if (adjust != Vector3.zero) _panel.position += adjust;
    }
}