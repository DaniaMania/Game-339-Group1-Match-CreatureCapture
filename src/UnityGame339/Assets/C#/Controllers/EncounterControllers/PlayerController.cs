using System.Collections;
using Game339.Shared;
using Game339.Shared.DependencyInjection;
using UnityEngine;

public class PlayerController : BattleController
{
    [SerializeField] private PlayerControllerView _playerControllerView;
    [Tooltip("Required. Player input is gated on this controller's OnPassivePhaseComplete event so passives play out before the player acts.")]
    [SerializeField] private PassivePhaseController _passivePhaseController;

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

        _passivePhaseController.OnPassivePhaseComplete += OnPassivePhaseComplete;
        _turnEngine.PlayerTurnEnd += OnPlayerTurnEnd;
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _playerControllerView.Deinitialize();
        _passivePhaseController.OnPassivePhaseComplete -= OnPassivePhaseComplete;
        _turnEngine.PlayerTurnEnd -= OnPlayerTurnEnd;
    }

    /// <summary>
    /// Called after the passive phase finishes — at this point the player can act.
    /// </summary>
    private void OnPassivePhaseComplete()
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

        if (arm.cooldownTurns > 0)
        {
            _armCooldowns[armIndex] = arm.cooldownTurns + 1;
        }

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