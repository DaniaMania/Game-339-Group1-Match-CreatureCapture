using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ButtonAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover")]
    [SerializeField] private float _hoverScale    = 1.1f;
    [SerializeField] private float _hoverDuration = 0.15f;
    [SerializeField] private float _wiggleAngle   = 6f;
    [SerializeField] private float _wiggleDuration = 0.4f;

    [Header("Click")]
    [SerializeField] private float _clickPunch    = 0.18f;
    [SerializeField] private float _clickDuration = 0.25f;
    [SerializeField] private int   _clickVibrato  = 6;

    private RectTransform _rt;
    private Vector3       _originalScale;
    private Tween         _scaleTween;
    private Tween         _wiggleTween;

    private void Awake()
    {
        _rt            = GetComponent<RectTransform>();
        _originalScale = _rt.localScale;
    }

    public void OnPointerEnter(PointerEventData _)
    {
        // Scale up
        _scaleTween?.Kill();
        _scaleTween = _rt
            .DOScale(_originalScale * _hoverScale, _hoverDuration)
            .SetEase(Ease.OutBack);

        // Rotation wiggle
        _wiggleTween?.Kill();
        _wiggleTween = _rt
            .DOPunchRotation(new Vector3(0f, 0f, _wiggleAngle), _wiggleDuration, 6, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData _)
    {
        _scaleTween?.Kill();
        _scaleTween = _rt
            .DOScale(_originalScale, _hoverDuration)
            .SetEase(Ease.OutBack);

        _wiggleTween?.Kill();
        _rt.DORotate(Vector3.zero, _hoverDuration);
    }

    public void OnPointerClick(PointerEventData _)
    {
        _scaleTween?.Kill();
        _wiggleTween?.Kill();

        _rt.DOPunchScale(Vector3.one * _clickPunch, _clickDuration, _clickVibrato, 0.5f)
            .OnComplete(() => _rt.localScale = _originalScale * _hoverScale);
    }

    private void OnDestroy()
    {
        _rt.DOKill();
    }
}