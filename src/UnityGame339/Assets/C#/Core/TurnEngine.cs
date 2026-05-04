using System;
using Game339.Shared;

public class TurnEngine
{
    //===== Turn Events =====
    public event Action<Character, Character> EncounterSetup;
    public event Action EncounterStart;
    public event Action<bool> EncounterEnd;
    public event Action<int> PlayerTurnStart;
    public event Action PlayerTurnEnd;
    public event Action<int> EnemyTurnStart;
    public event Action EnemyTurnEnd;

    //===== Global Information =====
    public ObservableValue<bool> IsPlayerTurn { get; } = new ObservableValue<bool>();
    public bool isEncounterRunning { private set; get; } = false;

    //===== Encounter Information =====
    private int _turnIndex = 0;
    private ActionPair _firstTurns = new ActionPair();
    private ActionPair _secondTurns = new ActionPair();
    private ActionPair _currentTurns;
    
    public Character Player { get; private set; }
    public Character Enemy { get; private set; }

    private TurnState _currentState;
    public TurnState State
    {
        get => _currentState;
        set
        {
            if (!Player || !Enemy)
            {
                throw new NullReferenceException($"player and/or enemy have not been " +
                                                 $"setup for encounter, call {nameof(SetupForNewEncounter)}");
            } 
            
            switch (value)
            {
                case TurnState.EnterEncounter:
                    EnterEncounter();
                    EncounterStart?.Invoke();
                    break;
                case TurnState.StartTurn:
                    StartTurn();
                    break;
                case TurnState.EndTurn:
                    EndTurn();
                    break;
                case TurnState.ExitEncounter:
                    bool isPlayerWin = ExitEncounter();
                    EncounterEnd?.Invoke(isPlayerWin);
                    Player = Enemy = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    public void SetupForNewEncounter(Character player, Character enemy)
    {
        _turnIndex = 0;
        Player = player;
        Enemy = enemy;
        EncounterSetup?.Invoke(player, enemy);
    }
    
    //===== Encounter =====
    private void EnterEncounter()
    {
        isEncounterRunning = true;
        
        //resolve who goes first
        if (Enemy.Speed.Value > Player.Speed.Value)
        {
            _firstTurns.SetPair(StartEnemyTurn, EndEnemyTurn);
            _secondTurns.SetPair(StartPlayerTurn, EndPlayerTurn);
        }
        else
        {
            _firstTurns.SetPair(StartPlayerTurn, EndPlayerTurn);
            _secondTurns.SetPair(StartEnemyTurn, EndEnemyTurn);
        }

        _currentTurns = _firstTurns;
    }
    
    private bool ExitEncounter()
    {
        IsPlayerTurn.Value = false;
        isEncounterRunning = false;

        _firstTurns.Clear();
        _secondTurns.Clear();
        
        return Player.HP.Value != 0;
    }

    //===== Turns =====
    private void StartTurn()
    {
        _turnIndex += 1;
        _currentTurns.First?.Invoke();
    }

    private void EndTurn()
    {
        _currentTurns.Second?.Invoke();
        
        //switch turn order
        _currentTurns = _turnIndex % 2 == 0 ? _firstTurns : _secondTurns;
    }
    
    //===== Character Turns =====
    //-- Player --
    private void StartPlayerTurn()
    {
        IsPlayerTurn.Value = true;
        PlayerTurnStart?.Invoke(_turnIndex);
    }
    
    private void EndPlayerTurn()
    {
        IsPlayerTurn.Value = false;
        PlayerTurnEnd?.Invoke();
    }
    
    //-- Enemy --
    private void StartEnemyTurn()
    {
        EnemyTurnStart?.Invoke(_turnIndex);
    }

    private void EndEnemyTurn()
    {
        EnemyTurnEnd?.Invoke();
    }

    public class ActionPair
    {
        public Action First { get; private set; } 
        public Action Second { get; private set; }

        public void SetPair(Action first, Action second)
        {
            First = first;
            Second = second;
        }
        
        public void Clear()
        {
            First = Second = null;
        }
    }
}

public enum TurnState
{
    EnterEncounter,
    StartTurn,
    EndTurn,
    // PlayerTurnStart,
    // EnemyTurnStart,
    // EnemyTurnEnd,
    // PlayerTurnEnd,
    ExitEncounter
}