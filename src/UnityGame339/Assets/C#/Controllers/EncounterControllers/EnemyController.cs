using System.Collections;
using Game.Runtime;
using Game339.Shared.DependencyInjection;
using UnityEngine;

public class EnemyController : BattleController
{
    protected override void EncounterBegin()
    {
        _turnEngine.EnemyTurnStart += Attack;
        _turnEngine.EnemyTurnEnd += OnEnemyTurnEnd;
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _turnEngine.EnemyTurnStart -= Attack;
        _turnEngine.EnemyTurnEnd -= OnEnemyTurnEnd;
    }

    private void OnEnemyTurnEnd()
    {
        Enemy.TickStatuses();
    }

    //===== Abilities =====   
    public void Attack()
    {
        _logger.Info("[Enemy] Used Default Attack");
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