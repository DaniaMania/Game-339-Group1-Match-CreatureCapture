using System;
using Game.Runtime;
using UnityEngine;

public class PlayerController : BattleController
{
    [SerializeField] private PlayerControllerView _playerControllerView;
    
    protected override Character ControllerCharacter => Player;

    protected override void Subscribe()
    {
        _playerControllerView.AssignListeners(Attack, Heal);
        Player.OnCharacterTakeDamage += OnTakeDamage;
        Player.OnCharacterDeath += OnDeath;
    }

    protected override void Unsubscribe()
    {
        Player.OnCharacterTakeDamage -= OnTakeDamage;
        Player.OnCharacterDeath -= OnDeath;
    }

    protected override void OnTakeDamage(int amount)
    {
        //do code
        base.OnTakeDamage(amount);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    //===== Abilities =====
    public override void Attack()
    {
        AttackImplementation();
    }



    public void Heal()
    {
        int healed = Mathf.Min(Player.HP.Value + Player.HealAmount.Value, Player.MaxHP.Value);
        Player.HP.Value = healed;
        _turnEngine.State = TurnState.EndTurn;
    }
}