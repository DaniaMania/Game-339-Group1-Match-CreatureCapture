using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows badges for active status effects on a Character (Block, Weakness, Vulnerability).
/// Each badge is a GameObject the script enables/disables based on whether the value is > 0,
/// plus a TMP text the script writes the current value/duration into.
/// The block badge's icon Image flashes blue briefly when block is consumed.
/// </summary>
public class StatusEffectsView : TypedView<Character>
{
    [Header("Block")]
    [SerializeField] private GameObject _blockBadge;
    [SerializeField] private TextMeshProUGUI _blockValueText;
    [Tooltip("The Image inside the block badge to recolor on the flash animation.")]
    [SerializeField] private Image _blockBadgeIcon;
    [SerializeField] private Color _blockFlashColor = new Color(0.3f, 0.7f, 1f);
    [SerializeField] private float _blockFlashDuration = 0.3f;

    [Header("Weakness")]
    [SerializeField] private GameObject _weaknessBadge;
    [SerializeField] private TextMeshProUGUI _weaknessValueText;

    [Header("Vulnerability")]
    [SerializeField] private GameObject _vulnerabilityBadge;
    [SerializeField] private TextMeshProUGUI _vulnerabilityValueText;

    private Character _character;
    private int _lastBlockValue;
    private Coroutine _blockFlashCoroutine;
    private Color _blockBadgeBaseColor = Color.white;

    protected override void InitializeView(Character[] character)
    {
        _character = character[0];

        // Cache base color so the flash returns to the original tint.
        if (_blockBadgeIcon != null) _blockBadgeBaseColor = _blockBadgeIcon.color;

        // Snapshot BEFORE subscribing so the snapshot itself isn't treated as a change.
        _lastBlockValue = _character.Block.Value;

        _character.Block.ChangeEvent += OnBlockChange;
        _character.WeaknessDuration.ChangeEvent += OnWeaknessChange;
        _character.VulnerabilityDuration.ChangeEvent += OnVulnerabilityChange;

        // Apply current values without animation.
        SetBadge(_blockBadge, _blockValueText, _character.Block.Value);
        SetBadge(_weaknessBadge, _weaknessValueText, _character.WeaknessDuration.Value);
        SetBadge(_vulnerabilityBadge, _vulnerabilityValueText, _character.VulnerabilityDuration.Value);
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

        if (_blockFlashCoroutine != null)
        {
            StopCoroutine(_blockFlashCoroutine);
            _blockFlashCoroutine = null;
        }
        if (_blockBadgeIcon != null) _blockBadgeIcon.color = _blockBadgeBaseColor;

        if (_blockBadge != null) _blockBadge.SetActive(false);
        if (_weaknessBadge != null) _weaknessBadge.SetActive(false);
        if (_vulnerabilityBadge != null) _vulnerabilityBadge.SetActive(false);
    }

    private void OnBlockChange(int value)
    {
        int delta = value - _lastBlockValue;
        _lastBlockValue = value;

        SetBadge(_blockBadge, _blockValueText, value);

        if (delta < 0 && _blockBadgeIcon != null)
        {
            if (_blockFlashCoroutine != null) StopCoroutine(_blockFlashCoroutine);
            _blockFlashCoroutine = StartCoroutine(FlashBlockBadge());
        }
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

    private IEnumerator FlashBlockBadge()
    {
        float half = _blockFlashDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / half);
            _blockBadgeIcon.color = Color.Lerp(_blockBadgeBaseColor, _blockFlashColor, p);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / half);
            _blockBadgeIcon.color = Color.Lerp(_blockFlashColor, _blockBadgeBaseColor, p);
            yield return null;
        }
        _blockBadgeIcon.color = _blockBadgeBaseColor;
        _blockFlashCoroutine = null;
    }
}