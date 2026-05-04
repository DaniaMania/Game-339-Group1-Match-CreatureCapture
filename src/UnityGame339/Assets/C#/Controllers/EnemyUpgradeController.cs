using Game.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyUpgradeController : Controller
{
    [SerializeField] private int _attackUpgradeAmount = 1;
    [SerializeField] private int _maxHPUpgradeAmount = 1;

    protected override void Subscribe()
    {
        _turnEngine.EncounterEnd += UpgradeEnemy;
        _turnEngine.EncounterStart += HealEnemy;
    }

    protected override void Unsubscribe()
    {
        _turnEngine.EncounterEnd -= UpgradeEnemy;
        _turnEngine.EncounterStart -= HealEnemy;
    }

    public void ResetEnemy()
    {
        Enemy.ResetValues();
    }

    private void UpgradeEnemy(bool isPlayerWin)
    {
        if (!isPlayerWin) return;
        
        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                Enemy.Attack.Value += _attackUpgradeAmount;
                Debug.Log($"enemy upgraded attack by {_attackUpgradeAmount}");
                break;
            case 1:
                Enemy.MaxHP.Value += _maxHPUpgradeAmount;
                Debug.Log($"enemy upgraded max health by {_maxHPUpgradeAmount}");
                break;
        }
    }

    private void HealEnemy()
    {
        _attackService.HealToFull(Enemy);
    }
}