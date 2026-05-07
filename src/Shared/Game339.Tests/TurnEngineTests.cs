using Game339.Shared.Models;
using Game339.Shared.Services;

namespace Game339.Tests;

public class TurnEngineTests
{
    private static MockCharacter CreateCharacter(int health, int damage, int speed)
    {
        MockCharacter c = new MockCharacter();
        c.HP.Value = health;
        c.Attack.Value = damage;
        c.Speed.Value = speed;
        return c;
    }

    [Test]
    public void TestPlayerWin()
    {
        MockCharacter player = CreateCharacter(100, 50, 1);
        MockCharacter enemy = CreateCharacter(100, 10, 0);

        MockEncounter(player, enemy, Check);
        return;

        void Check(bool b)
        {
            Assert.That(b);
        }
    } 
    
    [Test]
    public void TestEnemyWin()
    {
        MockCharacter player = CreateCharacter(100, 50, 0);
        MockCharacter enemy = CreateCharacter(100, 50, 1);

        MockEncounter(player, enemy, Check);
        return;

        void Check(bool b)
        {
            Assert.That(!b);
        }
    }

    private void MockEncounter(MockCharacter player, MockCharacter enemy, Action<bool> onCheck)
    {
        AttackService attackService = new AttackService();
        
        TurnEngine turnEngine = new TurnEngine();
        turnEngine.EncounterEnd += onCheck;
        
        turnEngine.SetupForNewEncounter(player, enemy);
        turnEngine.State = TurnState.EnterEncounter;

        while (!player.HasDied && !enemy.HasDied)
        {
            turnEngine.State = TurnState.StartTurn;

            MockCharacter attacker, target;
            if (turnEngine.TurnIndex % 2 == 0)
            {
                attacker = player;
                target = enemy;
            }
            else
            {
                attacker = enemy;
                target = player;
            }
            attackService.Attack(attacker, target);
            turnEngine.State = TurnState.EndTurn;
        }

        turnEngine.State = TurnState.ExitEncounter;

        turnEngine.EncounterEnd -= onCheck;
    }
}