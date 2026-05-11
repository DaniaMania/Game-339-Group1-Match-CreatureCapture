using TMPro;
using UnityEngine;

/// <summary>
/// Shows badges for active status effects on a Character (Block, Weakness, Vulnerability).
/// Each badge is a GameObject the script enables/disables based on whether the value is > 0,
/// plus a TMP text the script writes the current value/duration into.
/// </summary>
public class StatusEffectsView : TypedView<Character>
{
    [Header("Block")]
    [SerializeField] private GameObject _blockBadge;
    [SerializeField] private TextMeshProUGUI _blockValueText;

    [Header("Weakness")]
    [SerializeField] private GameObject _weaknessBadge;
    [SerializeField] private TextMeshProUGUI _weaknessValueText;

    [Header("Vulnerability")]
    [SerializeField] private GameObject _vulnerabilityBadge;
    [SerializeField] private TextMeshProUGUI _vulnerabilityValueText;

    private Character _character;

    protected override void InitializeView(Character[] character)
    {
        _character = character[0];

        _character.Block.ChangeEvent += OnBlockChange;
        _character.WeaknessDuration.ChangeEvent += OnWeaknessChange;
        _character.VulnerabilityDuration.ChangeEvent += OnVulnerabilityChange;

        // Snap to current values.
        OnBlockChange(_character.Block.Value);
        OnWeaknessChange(_character.WeaknessDuration.Value);
        OnVulnerabilityChange(_character.VulnerabilityDuration.Value);
    }

    protected override void DeinitializeView()
    {
        if (_character != null)
        {
            _character.Block.ChangeEvent -= OnBlockChange;
            _character.WeaknessDuration.ChangeEvent -= OnWeaknessChange;
            _character.VulnerabilityDuration.ChangeEvent -= OnVulnerabilityChange;
        }
        _character = null;

        // Hide everything in case the next character reuses this view.
        if (_blockBadge != null) _blockBadge.SetActive(false);
        if (_weaknessBadge != null) _weaknessBadge.SetActive(false);
        if (_vulnerabilityBadge != null) _vulnerabilityBadge.SetActive(false);
    }

    private void OnBlockChange(int value)
    {
        SetBadge(_blockBadge, _blockValueText, value);
    }

    private void OnWeaknessChange(int value)
    {
        SetBadge(_weaknessBadge, _weaknessValueText, value);
    }

    private void OnVulnerabilityChange(int value)
    {
        SetBadge(_vulnerabilityBadge, _vulnerabilityValueText, value);
    }

    private static void SetBadge(GameObject badge, TextMeshProUGUI text, int value)
    {
        if (badge != null) badge.SetActive(value > 0);
        if (text != null) text.text = value.ToString();
    }
}
