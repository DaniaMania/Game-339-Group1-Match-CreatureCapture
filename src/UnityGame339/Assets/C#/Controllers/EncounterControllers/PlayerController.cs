using UnityEngine;

public class PlayerController : BattleController
{
    [SerializeField] private PlayerControllerView _playerControllerView;
    
    protected override Character ControllerCharacter => Player;

    protected override void Subscribe()
    {
        base.Subscribe();
        _playerControllerView.AssignListeners(Attack, Heal);
    }
    
    protected override void Unsubscribe()
    {
        base.Unsubscribe();
    }

    protected override void OnTakeDamage(int amount)
    {
        //do code...
        EndTurn();
    }
    
    protected override void OnDeath()
    {
        //do code...
       ExitEncounter();
    }

    //===== Abilities ===== 
    public void Attack()
    {
        AttackImplementation();
        // EndTurn();
    }
    
    public void Heal()
    {
        int healed = Mathf.Min(Player.HP.Value + Player.HealAmount.Value, Player.MaxHP.Value);
        Player.HP.Value = healed;
        EndTurn();
    }
}