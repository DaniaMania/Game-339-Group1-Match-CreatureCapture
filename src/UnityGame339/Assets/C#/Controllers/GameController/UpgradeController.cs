using Game.Runtime;
using Game339.Shared;
using UnityEngine;

public class UpgradeController : GameController
{
    [SerializeField] private CharacterView _characterUpgradeView;
    [SerializeField] private UpgradeView _upgradeView;
    [Space]
    [SerializeField] private int _attackUpgradeAmount = 1;
    [SerializeField] private int _healPotencyUpgradeAmount = 1;

    public ObservableValue<bool> IsUpgradeAvailable { get; } = new ObservableValue<bool>();

    protected override void Subscribe()
    {
        _turnEngine.EncounterEnd += OnEncounterEnd;
        _upgradeView.Initialize(this);
        _characterUpgradeView.Initialize(Player);
    }

    protected override void Unsubscribe()
    {
        _turnEngine.EncounterEnd -= OnEncounterEnd;
        _upgradeView.Deinitialize();
        _characterUpgradeView.Deinitialize();
    }

    private void OnEncounterEnd(bool playerWon)
    {
        if (!playerWon) return;
        IsUpgradeAvailable.Value = true;
    }

    // called by UpgradeView button
    public void UpgradeAttack()
    {
        _logger.Info("[Upgrade] Attack Upgraded");
        
        if (!IsUpgradeAvailable.Value) return;
        Player.Attack.Value += _attackUpgradeAmount;
        StartNextEncounter();
    }

    // called by UpgradeView button
    public void HealToFull()
    {
        _logger.Info("[Upgrade] Healed to Full");
        
        if (!IsUpgradeAvailable.Value) return;
        Player.HealToFull();
        StartNextEncounter();
    }

    // called by UpgradeView button
    public void UpgradeHealPotency()
    {
        _logger.Info("[Upgrade] Heal Potency Increase");
        
        if (!IsUpgradeAvailable.Value) return;
        Player.HealAmount.Value += _healPotencyUpgradeAmount;
        StartNextEncounter();
    }

    private void StartNextEncounter()
    {
        IsUpgradeAvailable.Value = false;
        EncounterManager.Instance.BeginNewEncounter();
    }
}