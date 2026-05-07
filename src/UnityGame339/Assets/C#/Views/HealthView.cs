using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : TypedView<Character>
{
    [SerializeField] private TextMeshProUGUI _currentHealthText;
    [SerializeField] private TextMeshProUGUI _maxHealthText;
    [SerializeField] private Slider _healthBar;

    private Character _character;
    
    protected override void InitializeView(Character[] character)
    {
        _character = character[0];
        _character.HP.ChangeEvent += OnHealthChange;
        _character.MaxHP.ChangeEvent += OnMaxHealthChange;
        
        OnMaxHealthChange(_character.MaxHP.Value);
        OnHealthChange(_character.HP.Value);
    }
    
    protected override void DeinitializeView()
    {
        _character.HP.ChangeEvent -= OnHealthChange;
        _character.MaxHP.ChangeEvent -= OnMaxHealthChange;
    }

    private void OnHealthChange(int value)
    {
        _healthBar.value = value;
        _currentHealthText.text = $"{value}";
    }

    private void OnMaxHealthChange(int value)
    {
        _healthBar.maxValue = value;
        _maxHealthText.text = $"{value}";
    }
}