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
        _attackService.Attack(Player, Enemy);
        End();
    }

    public void UseArm(int armIndex)
    {
        BodyPart[] arms = Player.Loadout.arms;
        if (armIndex < 0 || armIndex >= arms.Length) return;
        BodyPart arm = arms[armIndex];
        if (arm == null) return;
        if (_armCooldowns[armIndex] > 0) return;

        // Lock input immediately so the player can't click anything during multihit playback.
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

        _armCooldowns[armIndex] = arm.cooldownTurns;
        _playerControllerView.RefreshArmCooldowns(_armCooldowns);
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