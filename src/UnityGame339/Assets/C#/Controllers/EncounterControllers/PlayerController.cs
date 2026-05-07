using UnityEngine;

public class PlayerController : BattleController
{
    [SerializeField] private PlayerControllerView _playerControllerView;

    protected override void EncounterBegin()
    {
       _playerControllerView.Initialize(this);
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _playerControllerView.Deinitialize();
    }

    //===== Abilities ===== 
    public void Attack()
    {
        _attackService.Attack(Player, Enemy);
        EndTurn();
    }

    public void Heal()
    {
        int healed = Mathf.Min(Player.HP.Value + Player.HealAmount.Value, Player.MaxHP.Value);
        Player.HP.Value = healed;
        EndTurn();
    }
}