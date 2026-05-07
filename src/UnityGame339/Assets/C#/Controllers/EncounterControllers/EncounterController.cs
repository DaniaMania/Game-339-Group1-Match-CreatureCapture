using Game.Runtime;

/// <summary>
/// change/update views over the course of an encounter.
/// </summary>
public abstract class EncounterController : Controller
{
   protected readonly AttackService _attackService = ServiceResolver.Resolve<AttackService>();

   protected Character Player { get; private set; }
   protected Character Enemy { get; private set; }
   
   protected override void Subscribe()
   {
      _turnEngine.EncounterSetup += StartNewEncounter;
      _turnEngine.EncounterEnd += EndNewEncounter;
   }

   protected override void Unsubscribe()
   {
      _turnEngine.EncounterSetup -= StartNewEncounter;
      _turnEngine.EncounterEnd -= EndNewEncounter;
      
      if (_turnEngine.IsEncounterRunning) EndNewEncounter(false);
   }

   private void StartNewEncounter(Character player, Character enemy)
   {
      Player = player;
      Enemy = enemy;
      EncounterBegin();
   }

   private void EndNewEncounter(bool isPlayerWin)
   { 
      EncounterEnd(isPlayerWin);
      Player = Enemy = null;
   }

   protected abstract void EncounterBegin();
   protected abstract void EncounterEnd(bool isPlayerWin);
}

public abstract class BattleController : EncounterController
{
   protected abstract Character ControllerCharacter { get; }
   private Character ControllerEnemyCharacter => (ControllerCharacter == Player) ? Enemy : Player;
   
   protected void AttackImplementation() => _attackService.Attack(ControllerCharacter, ControllerEnemyCharacter);

   protected override void EncounterBegin()
   {
      ControllerCharacter.OnCharacterTakeDamage += OnTakeDamage;
      ControllerCharacter.OnCharacterDeath += OnDeath;
   }

   protected override void EncounterEnd(bool isPlayerWin)
   {
      if (!ControllerCharacter) return;
      
      ControllerCharacter.OnCharacterTakeDamage -= OnTakeDamage;
      ControllerCharacter.OnCharacterDeath -= OnDeath; 
   }

   protected abstract void OnTakeDamage(int amount);
   protected abstract void OnDeath();

   protected void EndTurn() => _turnEngine.State = TurnState.EndTurn;
   protected void ExitEncounter() => _turnEngine.State = TurnState.ExitEncounter;
}

//todo: make this listen for the death callback and have EndTurn act as a signal for the actions being done 
// rather than the key to end the turn 