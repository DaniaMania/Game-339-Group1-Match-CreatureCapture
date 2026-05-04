using System;
using System.Collections;
using Game.Runtime;
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
   
   public void BeginNewEncounter()
   {
      //todo: pick a new enemy from the list in a good way (maybe by difficultly or by predetermined order)
      Character randomEnemy = CharacterDatabase.GetRandomCharacter();
      _turnEngine.SetupForNewEncounter(CharacterDatabase.PlayerCharacter, randomEnemy);
      _turnEngine.State = TurnState.EnterEncounter;
   }

   public void StartEncounter()
   {
      if (!_turnEngine.isEncounterRunning)
      {
         Debug.LogWarning("Encounter was not started");
         return;
      }

      _turnEngine.State = TurnState.StartTurn;
   }

   private void NextTurn()
   {
      StartCoroutine(Delay());
      return;
      IEnumerator Delay()
      {
         yield return new WaitForSeconds(0.1f);
         _turnEngine.State = TurnState.StartTurn;
      }
   }

   private new IEnumerator Start()
   {
      base.Start();
      
      yield return new WaitForSeconds(1f);
      BeginNewEncounter();
      yield return new WaitForSeconds(1f);
      StartEncounter();
   }

   protected override void Subscribe()
   {
      _turnEngine.EnemyTurnEnd += NextTurn;
      _turnEngine.PlayerTurnEnd += NextTurn;
   }

   protected override void Unsubscribe()
   {
      _turnEngine.EnemyTurnEnd -= NextTurn;
      _turnEngine.PlayerTurnEnd -= NextTurn;
   }
   
}
