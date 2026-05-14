using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI abilityLabel;

    [Header("Cooldown")]
    [SerializeField] private GameObject cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownLabel;

    public event Action<BodyPart> OnActionChosen;

    private BodyPart _sourcePart;
    private Character _owner;
    private int _currentCooldown;
    private bool _outerInteractable = true;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(() => OnActionChosen?.Invoke(_sourcePart));
    }

    public void Populate(BodyPart part, Character owner)
    {
        _sourcePart = part;
        _owner = owner;
        gameObject.SetActive(part != null);
        if (part == null) return;
        if (abilityLabel != null) abilityLabel.text = part.abilityName;
        SetCooldown(0);
    }

    public void SetInteractable(bool interactable)
    {
        _outerInteractable = interactable;
        RefreshInteractable();
    }

    public void SetCooldown(int turnsRemaining)
    {
        _currentCooldown = Mathf.Max(0, turnsRemaining);
        if (cooldownOverlay != null) cooldownOverlay.SetActive(_currentCooldown > 0);
        if (cooldownLabel != null) cooldownLabel.text = _currentCooldown > 0 ? _currentCooldown.ToString() : "";
        RefreshInteractable();
    }

    private void RefreshInteractable()
    {
        button.interactable = _outerInteractable && _currentCooldown == 0;
    }

    // ===== Tooltip =====

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_sourcePart == null || _owner == null) return;
        if (TooltipUI.Instance == null) return;

        string effect = BodyPartFormatter.FormatEffect(_sourcePart, _owner);
        string cooldownInfo = BodyPartFormatter.FormatCooldown(_sourcePart, _currentCooldown);

        TooltipUI.Instance.Show(
            _sourcePart.abilityName,
            effect,
            _sourcePart.abilityDescription,
            (RectTransform)transform,
            cooldownInfo,
            _sourcePart.icon);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance == null) return;
        TooltipUI.Instance.Hide();
    }
}