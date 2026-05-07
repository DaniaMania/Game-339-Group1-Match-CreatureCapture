using Game.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyUpgradeController : EncounterController
{
    [SerializeField] private int _attackUpgradeAmount = 1;
    [SerializeField] private int _maxHPUpgradeAmount = 1;
    
    protected override void EncounterBegin()
    {
        Enemy.HealToFull();
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        UpgradeEnemy(isPlayerWin);
    }

    private void UpgradeEnemy(bool isPlayerWin)
    {
        if (!isPlayerWin) return;
        
        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                Enemy.Attack.Value += _attackUpgradeAmount;
                // Debug.Log($"enemy upgraded attack by {_attackUpgradeAmount}");
                break;
            case 1:
                Enemy.MaxHP.Value += _maxHPUpgradeAmount;
                // Debug.Log($"enemy upgraded max health by {_maxHPUpgradeAmount}");
                break;
        }
    }
    
    public void ResetEnemy()
    {
        Enemy.ResetValues();
    }
}