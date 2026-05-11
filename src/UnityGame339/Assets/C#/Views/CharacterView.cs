using System.Collections;
using Game.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : TypedView<Character>
{
    [Header("Values")]
    [SerializeField] private Image _characterImage;
    
    [Header("Sub-views")]
    [SerializeField] private HealthView _healthView;
    [SerializeField] private StatusEffectsView _statusEffectsView;

    [Header("Glow (driven by TurnIndicatorView)")]
    [Tooltip("A child GameObject (outline, aura, halo, etc.) that gets activated on this character's turn.")]
    [SerializeField] private GameObject _glowObject;

    [Header("Hit Feedback Animation")]
    [SerializeField] private float _hitFeedbackDuration = 0.3f;
    [SerializeField] private float _hitTiltAngle = 12f;

    [Header("Hit Feedback Colors")]
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private Color _healColor = new Color(0.3f, 1f, 0.4f);
    [SerializeField] private Color _blockColor = new Color(0.3f, 0.7f, 1f);
    [SerializeField] private Color _weaknessColor = new Color(0.7f, 0.85f, 1f);
    [SerializeField] private Color _vulnerabilityColor = new Color(1f, 0.7f, 0.7f);

    [Header("Floating Numbers")]
    [SerializeField] private RectTransform _floatingNumbersContainer;
    [SerializeField] private FloatingNumber _floatingNumberPrefab;

    private Character _character;
    private Coroutine _hitFeedbackCoroutine;
    private Color _baseColor = Color.white;

    // Delta tracking — set in InitializeView, updated on each change event.
    private int _lastHP;
    private int _lastBlock;
    private int _lastWeakness;
    private int _lastVulnerability;
    
    protected override void InitializeView(Character[] character)
    {
        _character = character[0];
        _characterImage.sprite = _character.Icon;
        _baseColor = _characterImage.color;

        // Snapshot current values BEFORE subscribing so the snapshot itself doesn't trigger feedback.
        _lastHP = _character.HP.Value;
        _lastBlock = _character.Block.Value;
        _lastWeakness = _character.WeaknessDuration.Value;
        _lastVulnerability = _character.VulnerabilityDuration.Value;

        _character.OnCharacterTakeDamage += OnTakeDamage;
        _character.HP.ChangeEvent += OnHPChange;
        _character.Block.ChangeEvent += OnBlockChange;
        _character.WeaknessDuration.ChangeEvent += OnWeaknessChange;
        _character.VulnerabilityDuration.ChangeEvent += OnVulnerabilityChange;

        _healthView.Initialize(character);
        if (_statusEffectsView != null) _statusEffectsView.Initialize(character);
    }

    protected override void DeinitializeView()
    {
        if (_character != null)
        {
            _character.OnCharacterTakeDamage -= OnTakeDamage;
            _character.HP.ChangeEvent -= OnHPChange;
            _character.Block.ChangeEvent -= OnBlockChange;
            _character.WeaknessDuration.ChangeEvent -= OnWeaknessChange;
            _character.VulnerabilityDuration.ChangeEvent -= OnVulnerabilityChange;
        }
        _character = null;

        if (_hitFeedbackCoroutine != null)
        {
            StopCoroutine(_hitFeedbackCoroutine);
            _hitFeedbackCoroutine = null;
        }
        if (_characterImage)
        {
            _characterImage.color = _baseColor;
            _characterImage.transform.localEulerAngles = Vector3.zero;
            _characterImage.sprite = null;
        }

        SetGlow(false);

        _healthView.Deinitialize();
        if (_statusEffectsView != null) _statusEffectsView.Deinitialize();
    }

    //===== Glow (toggled by TurnIndicatorView) =====

    public void SetGlow(bool active)
    {
        if (_glowObject != null) _glowObject.SetActive(active);
    }

    //===== Event handlers =====

    private void OnTakeDamage(int amount)
    {
        PlayFlash(_damageColor, withTilt: true);
        SpawnFloatingNumber($"-{amount}", _damageColor);
    }

    private void OnHPChange(int newValue)
    {
        int delta = newValue - _lastHP;
        _lastHP = newValue;
        if (delta > 0)
        {
            PlayFlash(_healColor, withTilt: false);
            SpawnFloatingNumber($"+{delta}", _healColor);
        }
    }

    private void OnBlockChange(int newValue)
    {
        int delta = newValue - _lastBlock;
        _lastBlock = newValue;
        if (delta == 0) return;

        if (delta < 0)
        {
            PlayFlash(_blockColor, withTilt: false);
            SpawnFloatingNumber($"-{-delta}", _blockColor);
        }
        else
        {
            SpawnFloatingNumber($"+{delta}", _blockColor);
        }
    }

    private void OnWeaknessChange(int newValue)
    {
        int delta = newValue - _lastWeakness;
        _lastWeakness = newValue;
        if (delta > 0) PlayFlash(_weaknessColor, withTilt: false);
    }

    private void OnVulnerabilityChange(int newValue)
    {
        int delta = newValue - _lastVulnerability;
        _lastVulnerability = newValue;
        if (delta > 0) PlayFlash(_vulnerabilityColor, withTilt: false);
    }

    //===== Public manual triggers =====

    public void PlayDamageFlash() => PlayFlash(_damageColor, withTilt: true);
    public void PlayHealFlash() => PlayFlash(_healColor, withTilt: false);
    public void PlayBlockSoakFlash() => PlayFlash(_blockColor, withTilt: false);
    public void PlayWeaknessFlash() => PlayFlash(_weaknessColor, withTilt: false);
    public void PlayVulnerabilityFlash() => PlayFlash(_vulnerabilityColor, withTilt: false);

    //===== Animation =====

    private void PlayFlash(Color color, bool withTilt)
    {
        if (_hitFeedbackCoroutine != null) StopCoroutine(_hitFeedbackCoroutine);
        _hitFeedbackCoroutine = StartCoroutine(FlashCoroutine(color, withTilt));
    }

    private IEnumerator FlashCoroutine(Color color, bool withTilt)
    {
        if (_characterImage == null) yield break;

        Transform t = _characterImage.transform;
        float half = _hitFeedbackDuration * 0.5f;

        float elapsed = 0;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / half);
            _characterImage.color = Color.Lerp(_baseColor, color, p);
            if (withTilt) t.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0f, -_hitTiltAngle, p));
            yield return null;
        }

        elapsed = 0;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / half);
            _characterImage.color = Color.Lerp(color, _baseColor, p);
            if (withTilt) t.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-_hitTiltAngle, 0f, p));
            yield return null;
        }

        _characterImage.color = _baseColor;
        t.localEulerAngles = Vector3.zero;
        _hitFeedbackCoroutine = null;
    }

    private void SpawnFloatingNumber(string content, Color color)
    {
        if (_floatingNumberPrefab == null || _floatingNumbersContainer == null) return;
        FloatingNumber instance = Instantiate(_floatingNumberPrefab, _floatingNumbersContainer);
        instance.transform.localPosition = Vector3.zero;
        instance.Play(content, color);
    }
}