using Game339.Shared;
using TMPro;
using UnityEngine;

public class BaseStatsView : TypedView<Character>
{
    [SerializeField] private TextMeshProUGUI _maxHPText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _healAmountText;

    private Character _character;

    protected override void InitializeView(Character[] character)
    {
        _character = character[0];

        _character.MaxHP.ChangeEvent      += OnMaxHPChanged;
        _character.Attack.ChangeEvent     += OnAttackChanged;
        _character.Defense.ChangeEvent    += OnDefenseChanged;
        _character.Speed.ChangeEvent      += OnSpeedChanged;
        _character.HealAmount.ChangeEvent += OnHealAmountChanged;
    }

    protected override void DeinitializeView()
    {
        _character.MaxHP.ChangeEvent      -= OnMaxHPChanged;
        _character.Attack.ChangeEvent     -= OnAttackChanged;
        _character.Defense.ChangeEvent    -= OnDefenseChanged;
        _character.Speed.ChangeEvent      -= OnSpeedChanged;
        _character.HealAmount.ChangeEvent -= OnHealAmountChanged;
        _character = null;
    }

    private void OnMaxHPChanged(int v)      => SetText(_maxHPText,     $"HP:   {v}");
    private void OnAttackChanged(int v)     => SetText(_attackText,    $"ATK:  {v}");
    private void OnDefenseChanged(int v)    => SetText(_defenseText,   $"DEF:  {v}");
    private void OnSpeedChanged(int v)      => SetText(_speedText,     $"SPD:  {v}");
    private void OnHealAmountChanged(int v) => SetText(_healAmountText,$"HEAL: {v}");

    private void SetText(TextMeshProUGUI label, string value)
    {
        if (label != null) label.text = value;
    }
}