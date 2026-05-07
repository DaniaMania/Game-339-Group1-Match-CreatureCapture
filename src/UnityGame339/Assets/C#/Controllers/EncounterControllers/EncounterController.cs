using System;
using System.Collections;
using Game.Runtime;
using UnityEngine;

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

   /// <summary>
   /// when an encounter begins, setup and view initialization should be done here
   /// </summary>
   protected abstract void EncounterBegin();
   
   /// <summary>
   /// when an encounter ends, deinitialization should be here
   /// </summary>
   /// <param name="isPlayerWin"></param>
   protected abstract void EncounterEnd(bool isPlayerWin);
}

public abstract class BattleController : EncounterController
{
   //--IMPORTANT-- 
   //all animations must finish within the times below otherwise they will not be shown in full
   private const float TURN_TIME = 0.6f;
   private const float DEATH_TIME = 0.8f;
   
   /// <summary>
   /// changes the TurnEngine state after a short delay 
   /// </summary>
   protected void EndTurn()
   {
      if (Player.HasDied || Enemy.HasDied)
      {
         StartCoroutine(Delay(DEATH_TIME, () => _turnEngine.State = TurnState.ExitEncounter));
      }
      else
      {
         StartCoroutine(Delay(TURN_TIME, () => _turnEngine.State = TurnState.EndTurn));
      }
   }

   private static IEnumerator Delay(float delayTime, Action onComplete)
   {
      yield return new WaitForSeconds(delayTime);
      onComplete.Invoke();
   }
}