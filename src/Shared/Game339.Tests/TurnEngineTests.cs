using Game339.Shared.DependencyInjection;
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

    [Test]
    public void TestEvents()
    {
        int eventCounter = 0;
        AttackService attackService = new AttackService();
        
        MockCharacter player = CreateCharacter(100, 50, 1);
        MockCharacter enemy = CreateCharacter(100, 90, 0);
        
        TurnEngine turnEngine = new TurnEngine();
        turnEngine.EncounterSetup += EncounterSetup;
        turnEngine.EncounterStart += EncounterStart;
        turnEngine.PlayerTurnStart += PlayerTurnStart;
        turnEngine.PlayerTurnEnd += PlayerTurnEnd;
        turnEngine.EnemyTurnStart += EnemyTurnStart;
        turnEngine.EnemyTurnEnd += EnemyTurnEnd;
        turnEngine.EncounterEnd += EncounterEnd;
        
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

        turnEngine.EncounterSetup -= EncounterSetup;
        turnEngine.EncounterStart -= EncounterStart;
        turnEngine.PlayerTurnStart -= PlayerTurnStart;
        turnEngine.PlayerTurnEnd -= PlayerTurnEnd;
        turnEngine.EnemyTurnStart -= EnemyTurnStart;
        turnEngine.EnemyTurnEnd -= EnemyTurnEnd;
        turnEngine.EncounterEnd -= EncounterEnd;

        Assert.That(eventCounter, Is.EqualTo(9));
        return;

        void EncounterSetup(ICharacter p, ICharacter e)
        {
            eventCounter++;
        }

        void EncounterStart()
        {
            eventCounter++;
        }

        void PlayerTurnStart()
        {
            eventCounter++;
        }

        void PlayerTurnEnd()
        {
            eventCounter++;
        }

        void EnemyTurnStart()
        {
            eventCounter++;
        }

        void EnemyTurnEnd()
        {
            eventCounter++;
        }

        void EncounterEnd(bool isPlayerWin)
        {
            eventCounter++;
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