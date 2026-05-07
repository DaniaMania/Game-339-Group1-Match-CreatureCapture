using System.Collections;
using Game.Runtime;
using UnityEngine;

public class EnemyController : BattleController
{
   protected override Character ControllerCharacter => Enemy;

   protected override void Subscribe()
   {
      base.Subscribe();
      _turnEngine.PlayerTurnEnd += Attack;
   }

   protected override void Unsubscribe()
   {
      base.Unsubscribe();
      _turnEngine.PlayerTurnEnd -= Attack;
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
      StartCoroutine(AttackDelay());
      return;

      IEnumerator AttackDelay()
      {
         yield return new WaitForSeconds(0.8f);
         AttackImplementation();
         // EndTurn();
      }
   }
}
