using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : TypedView<Character>
{
    [SerializeField] private TextMeshProUGUI _currentHealthText;
    [SerializeField] private TextMeshProUGUI _maxHealthText;
    [SerializeField] private Slider _healthBar;

    [Header("Animation")]
    [SerializeField] private float _animationDuration = 0.35f;

    private Character _character;
    private Coroutine _animCoroutine;
    
    protected override void InitializeView(Character[] character)
    {
        _character = character[0];
        _character.HP.ChangeEvent += OnHealthChange;
        _character.MaxHP.ChangeEvent += OnMaxHealthChange;

        // Snap to initial values without animating in.
        OnMaxHealthChange(_character.MaxHP.Value);
        SnapHealth(_character.HP.Value);
    }
    
    protected override void DeinitializeView()
    {
        if (_character != null)
        {
            _character.HP.ChangeEvent -= OnHealthChange;
            _character.MaxHP.ChangeEvent -= OnMaxHealthChange;
        }

        if (_animCoroutine != null)
        {
            StopCoroutine(_animCoroutine);
            _animCoroutine = null;
        }
    }

    private void OnHealthChange(int value)
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(AnimateHealthTo(value));
    }

    private IEnumerator AnimateHealthTo(int targetValue)
    {
        float startValue = _healthBar.value;
        float elapsed = 0f;
        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _animationDuration);
            float v = Mathf.Lerp(startValue, targetValue, t);
            _healthBar.value = v;
            _currentHealthText.text = Mathf.RoundToInt(v).ToString();
            yield return null;
        }
        SnapHealth(targetValue);
        _animCoroutine = null;
    }

    private void SnapHealth(int value)
    {
        _healthBar.value = value;
        _currentHealthText.text = value.ToString();
    }

    private void OnMaxHealthChange(int value)
    {
        _healthBar.maxValue = value;
        _maxHealthText.text = value.ToString();
    }
}