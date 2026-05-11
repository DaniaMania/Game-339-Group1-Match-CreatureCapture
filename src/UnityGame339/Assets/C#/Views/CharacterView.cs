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

    [Header("Hit Feedback")]
    [SerializeField] private Color _hitFlashColor = Color.red;
    [SerializeField] private float _hitFeedbackDuration = 0.3f;
    [SerializeField] private float _hitTiltAngle = 12f;

    private Character _character;
    private Coroutine _hitFeedbackCoroutine;
    private Color _baseColor = Color.white;
    
    protected override void InitializeView(Character[] character)
    {
        _character = character[0];
        _characterImage.sprite = _character.Icon;
        _baseColor = _characterImage.color;

        _character.OnCharacterTakeDamage += OnTakeDamage;

        _healthView.Initialize(character);
        if (_statusEffectsView != null) _statusEffectsView.Initialize(character);
    }

    protected override void DeinitializeView()
    {
        if (_character != null) _character.OnCharacterTakeDamage -= OnTakeDamage;
        _character = null;

        // Reset visuals so the next encounter doesn't inherit a tilted/red sprite.
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

        _healthView.Deinitialize();
        if (_statusEffectsView != null) _statusEffectsView.Deinitialize();
    }

    private void OnTakeDamage(int _)
    {
        if (_hitFeedbackCoroutine != null) StopCoroutine(_hitFeedbackCoroutine);
        _hitFeedbackCoroutine = StartCoroutine(HitFeedback());
    }

    private IEnumerator HitFeedback()
    {
        if (_characterImage == null) yield break;

        Transform t = _characterImage.transform;
        float half = _hitFeedbackDuration * 0.5f;

        // Outgoing: tilt and flash to red.
        float elapsed = 0;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / half);
            _characterImage.color = Color.Lerp(_baseColor, _hitFlashColor, p);
            t.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0f, -_hitTiltAngle, p));
            yield return null;
        }

        // Return: settle back to neutral.
        elapsed = 0;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / half);
            _characterImage.color = Color.Lerp(_hitFlashColor, _baseColor, p);
            t.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-_hitTiltAngle, 0f, p));
            yield return null;
        }

        _characterImage.color = _baseColor;
        t.localEulerAngles = Vector3.zero;
        _hitFeedbackCoroutine = null;
    }
}