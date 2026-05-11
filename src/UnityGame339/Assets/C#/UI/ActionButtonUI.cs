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

        string effect = BuildEffectText(_sourcePart, _owner);
        string cooldownInfo = BuildCooldownText(_sourcePart, _currentCooldown);

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

    private static string BuildEffectText(BodyPart part, Character owner)
    {
        int hits = Mathf.Max(1, part.abilityHits);
        string effect;

        switch (part.abilityType)
        {
            case AbilityType.Attack:
            {
                int perHit = owner.Attack.Value;
                int total = perHit * hits;
                effect = hits > 1
                    ? $"Deal {perHit} damage x {hits} hits ({total} total)"
                    : $"Deal {perHit} damage";
                break;
            }
            case AbilityType.Heal:
            {
                int perHit = part.abilityValue;
                int total = perHit * hits;
                effect = hits > 1
                    ? $"Heal {perHit} HP x {hits} ({total} total)"
                    : $"Heal {perHit} HP";
                break;
            }
            case AbilityType.Shield:
            {
                int perHit = part.abilityValue;
                int total = perHit * hits;
                effect = hits > 1
                    ? $"Gain {perHit} block x {hits} ({total} total)"
                    : $"Gain {perHit} block";
                break;
            }
            case AbilityType.Weakness:
            {
                int totalDuration = part.abilityValue * hits;
                effect = $"Apply Weakness for {totalDuration} turn(s) (-25% target attack)";
                break;
            }
            case AbilityType.Vulnerability:
            {
                int totalDuration = part.abilityValue * hits;
                effect = $"Apply Vulnerability for {totalDuration} turn(s) (+50% damage taken)";
                break;
            }
            default:
                effect = "Unknown ability";
                break;
        }

        return effect;
    }

    private static string BuildCooldownText(BodyPart part, int currentCooldown)
    {
        if (part.cooldownTurns <= 0) return null;
        if (currentCooldown > 0)
        {
            return $"Cooldown: {part.cooldownTurns} turns ({currentCooldown} remaining)";
        }
        return $"Cooldown: {part.cooldownTurns} turns";
    }
}