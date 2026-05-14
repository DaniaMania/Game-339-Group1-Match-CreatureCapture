using System.Collections;
using Game339.Shared;
using Game339.Shared.DependencyInjection;
using UnityEngine;

public class PlayerController : BattleController
{
    [SerializeField] private PlayerControllerView _playerControllerView;

    [Header("Multihit")]
    [Tooltip("Seconds of delay between hits of a multihit ability so they're individually visible.")]
    [SerializeField] private float _multihitDelay = 0.3f;
    
    public readonly ObservableValue<bool> IsInteractable = new ObservableValue<bool>();

    // One cooldown counter per arm slot. 0 = available.
    private readonly int[] _armCooldowns = new int[2];
    
    protected override void EncounterBegin()
    {
        _playerControllerView.Initialize(this);
        _playerControllerView.PopulateArms(Player.Loadout.arms, Player);

        for (int i = 0; i < _armCooldowns.Length; i++) _armCooldowns[i] = 0;
        _playerControllerView.RefreshArmCooldowns(_armCooldowns);

        _turnEngine.PlayerTurnStart += OnPlayerTurnStart;
        _turnEngine.PlayerTurnEnd += OnPlayerTurnEnd;
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _playerControllerView.Deinitialize();
        _turnEngine.PlayerTurnStart -= OnPlayerTurnStart;
        _turnEngine.PlayerTurnEnd -= OnPlayerTurnEnd;
    }

    private void OnPlayerTurnStart()
    {
        IsInteractable.Value = true;
        _playerControllerView.RefreshArmCooldowns(_armCooldowns);
    }

    private void OnPlayerTurnEnd()
    {
        for (int i = 0; i < _armCooldowns.Length; i++)
        {
            if (_armCooldowns[i] > 0) _armCooldowns[i]--;
        }
        Player.TickStatuses();
        _playerControllerView.RefreshArmCooldowns(_armCooldowns);
    }

    //===== Abilities =====

    public void DefaultAttack()
    {
        _logger.Info("[Player] Used Default Attack");
        
        _attackService.Attack(Player, Enemy);
        End();
    }

    public void UseArm(int armIndex)
    {
        _logger.Info($"[Player] Used Arm {armIndex}");
        
        BodyPart[] arms = Player.Loadout.arms;
        if (armIndex < 0 || armIndex >= arms.Length) return;
        BodyPart arm = arms[armIndex];
        if (arm == null) return;
        if (_armCooldowns[armIndex] > 0) return;

        IsInteractable.Value = false;
        StartCoroutine(UseArmCoroutine(armIndex, arm));
    }

    private IEnumerator UseArmCoroutine(int armIndex, BodyPart arm)
    {
        int hits = Mathf.Max(1, arm.abilityHits);
        for (int i = 0; i < hits; i++)
        {
            ExecuteAbilityHit(arm);
            if (Enemy.HasDied) break;
            if (i < hits - 1) yield return new WaitForSeconds(_multihitDelay);
        }

        // Store cooldownTurns + 1 so that the immediate tick at OnPlayerTurnEnd lands
        // on cooldownTurns. That gives N full lockout turns matching the authored value.
        // Skipped entirely when cooldownTurns is 0 so no-cooldown abilities don't show
        // a "1" overlay flash before being ticked away.
        if (arm.cooldownTurns > 0)
        {
            _armCooldowns[armIndex] = arm.cooldownTurns + 1;
        }
        // No RefreshArmCooldowns here — the post-tick refresh in OnPlayerTurnEnd will
        // show the correct value, avoiding a brief flash of cooldownTurns+1 in the UI.

        End();
    }

    private void ExecuteAbilityHit(BodyPart arm)
    {
        switch (arm.abilityType)
        {
            case AbilityType.Attack:
                _attackService.Attack(Player, Enemy);
                break;
            case AbilityType.Heal:
                Player.Heal(arm.abilityValue);
                break;
            case AbilityType.Shield:
                Player.AddBlock(arm.abilityValue);
                break;
            case AbilityType.Weakness:
                Enemy.ApplyWeakness(arm.abilityValue);
                break;
            case AbilityType.Vulnerability:
                Enemy.ApplyVulnerability(arm.abilityValue);
                break;
        }
    }

    private void End()
    {
        IsInteractable.Value = false;
        EndTurn();
    }
}