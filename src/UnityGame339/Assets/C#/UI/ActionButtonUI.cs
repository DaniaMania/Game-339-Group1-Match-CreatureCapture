using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// One action button in the Battle UI action bar.
/// Represents the active ability of a body part.
///
/// Hierarchy expected:
///   ActionButton (this script + Button)
///     ├── AbilityLabel    (TextMeshProUGUI)
///     ├── DamageLabel     (TextMeshProUGUI — e.g. "10 dmg")
///     └── CooldownOverlay (GameObject — shown when on cooldown, future use)
public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI abilityLabel;
    [SerializeField] private TextMeshProUGUI damageLabel;
    [SerializeField] private GameObject cooldownOverlay;

    public event Action<BodyPart> OnActionChosen;

    private BodyPart _sourcePart;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(() => OnActionChosen?.Invoke(_sourcePart));
    }

    public void Populate(BodyPart part)
    {
        _sourcePart = part;
        gameObject.SetActive(part != null);
        if (part == null) return;

        if (abilityLabel != null) abilityLabel.text = part.abilityName;
        if (damageLabel  != null) damageLabel.text  = $"{part.abilityBaseDamage} dmg";
        if (cooldownOverlay != null) cooldownOverlay.SetActive(false);
    }

    public void SetInteractable(bool interactable) => button.interactable = interactable;

    // TODO: Show cooldown overlay and count when cooldown system is added
}