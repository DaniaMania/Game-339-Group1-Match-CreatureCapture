using Game339.Shared;
using UnityEngine;

public class PlayerController : BattleController
{
    [SerializeField] private PlayerControllerView _playerControllerView;
    
    public readonly ObservableValue<bool> IsInteractable = new ObservableValue<bool>();
    
    protected override void EncounterBegin()
    {
       _playerControllerView.Initialize(this);
       _turnEngine.PlayerTurnStart += SetInteractable;
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _playerControllerView.Deinitialize();
        _turnEngine.PlayerTurnStart -= SetInteractable;
    }

    private void SetInteractable()
    {
        IsInteractable.Value = true;
    }

    //===== Abilities ===== 
    public void Attack()
    {
        _attackService.Attack(Player, Enemy);
        End();
    }

    public void Heal()
    {
        int healed = Mathf.Min(Player.HP.Value + Player.HealAmount.Value, Player.MaxHP.Value);
        Player.HP.Value = healed;
        End();
    }

    private void End()
    {
        IsInteractable.Value = false;
        EndTurn();
    }
}