using System;
using System.Collections;
using Game.Runtime;
using Game339.Shared;
using UnityEngine;

public class EncounterManager : ObserverMonoBehaviour
{
   #region Singleton
   public static EncounterManager Instance;
   private new void Awake()
   {
      base.Awake();
      if (Instance == null) Instance = this;
      else
      {
         Destroy(gameObject);
         return;
      }
   }
   #endregion

   private TurnEngine _turnEngine = ServiceResolver.Resolve<TurnEngine>();

   protected override void Subscribe()
   {
      _turnEngine.EnemyTurnEnd += NextTurn;
      _turnEngine.PlayerTurnEnd += NextTurn;
      _turnEngine.EncounterEnd += EndEncounter;

      _turnEngine.IsPlayerTurn.Value = false;
   }

   protected override void Unsubscribe()
   {
      _turnEngine.EnemyTurnEnd -= NextTurn;
      _turnEngine.PlayerTurnEnd -= NextTurn;
      _turnEngine.EncounterEnd -= EndEncounter;
   }
   
   // ===== Encounter Logic =====
   public void BeginNewEncounter()
   {
      //todo: pick a new enemy from the list in a good way (maybe by difficulty or by predetermined order)
      Character randomEnemy = CharacterDatabase.GetRandomCharacter();
      CharacterDatabase.Instance.ResetCharacterValue(ref randomEnemy);

      // Clear any leftover combat state on the player from the previous encounter.
      // (Block/Weakness/Vulnerability/Thorns — HP and MaxHP are intentionally preserved,
      // since the upgrade flow already sets HP to full via HealToFull, and the lose-restart
      // path goes through ResetValues which covers everything.)
      // Done BEFORE SetupForNewEncounter so views observe the cleared state from the start.
      CharacterDatabase.PlayerCharacter.ResetCombatState();

      _turnEngine.SetupForNewEncounter(CharacterDatabase.PlayerCharacter, randomEnemy);
      _turnEngine.State = TurnState.EnterEncounter;
      
      StartCoroutine(Delay());
      return;

      IEnumerator Delay()
      {
         yield return new WaitForSeconds(0.75f);
         StartEncounter();
      }
   }

   public void StartEncounter()
   {
      if (!_turnEngine.IsEncounterRunning)
      {
         Debug.LogWarning("Encounter was not started");
         return;
      }

      _turnEngine.State = TurnState.StartTurn;
   }

   private void NextTurn()
   {
      //todo: this is where we will resolve all the animations for the damage and stats effects 
      StartCoroutine(Delay());
      return;
      IEnumerator Delay()
      {
         yield return new WaitForSeconds(0.1f);
         _turnEngine.State = TurnState.StartTurn;
      }
   }

   private void EndEncounter(bool _)
   {
     
   }

   //===== Other =====
   private new IEnumerator Start()
   {
      base.Start();
      
      yield return new WaitForSeconds(1f);
      BeginNewEncounter();
   }
}