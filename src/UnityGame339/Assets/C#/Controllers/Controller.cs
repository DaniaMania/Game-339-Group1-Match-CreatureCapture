using System;
using Game.Runtime;
using UnityEngine;

public abstract class Controller : ObserverMonoBehaviour
{
   protected readonly TurnEngine _turnEngine = ServiceResolver.Resolve<TurnEngine>();
   protected readonly AttackService _attackService = ServiceResolver.Resolve<AttackService>();

   protected Character Player => _turnEngine.Player;
   protected Character Enemy => _turnEngine.Enemy;
}

public abstract class BattleController : Controller
{
   protected abstract Character ControllerCharacter { get; }
   private Character ControllerEnemyCharacter => (ControllerCharacter == Player) ? Enemy : Player;

   public abstract void Attack();
   protected void AttackImplementation() => _attackService.Attack(ControllerCharacter, ControllerEnemyCharacter);

      // void TargetDead() => _turnEngine.State = TurnState.ExitEncounter;
      // void TargetAlive() => _turnEngine.State = TurnState.EndTurn;

      protected virtual void OnTakeDamage(int amount)
      {
         _turnEngine.State = TurnState.EndTurn;
      }
      
      protected virtual void OnDeath()
      {
         _turnEngine.State = TurnState.ExitEncounter;
      }
}
