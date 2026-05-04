using System.Collections;
using UnityEngine;

public class EnemyController : BattleController
{
   protected override Character ControllerCharacter => Enemy;

   protected override void Subscribe()
   {
      _turnEngine.PlayerTurnEnd += Attack;
   }

   protected override void Unsubscribe()
   {
      _turnEngine.PlayerTurnEnd -= Attack;
   }

   public override void Attack()
   {
      StartCoroutine(AttackDelay());
      return;

      IEnumerator AttackDelay()
      {
         yield return new WaitForSeconds(1.2f);
         AttackImplementation();
      }
   }
}
