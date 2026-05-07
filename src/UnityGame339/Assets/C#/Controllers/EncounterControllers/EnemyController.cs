using System.Collections;
using Game.Runtime;
using UnityEngine;

public class EnemyController : BattleController
{
   protected override void EncounterBegin()
   {
      _turnEngine.PlayerTurnEnd += Attack;
   }

   protected override void EncounterEnd(bool isPlayerWin)
   {
      _turnEngine.PlayerTurnEnd -= Attack;
   }

   //===== Abilities =====   
   public void Attack()
   {
      StartCoroutine(AttackDelay());
      return;

      IEnumerator AttackDelay()
      {
         yield return new WaitForSeconds(0.8f);
         _attackService.Attack(Enemy, Player);
         EndTurn();
      }
   }
}
