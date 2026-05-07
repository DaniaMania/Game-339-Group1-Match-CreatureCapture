using System;
using Game339.Shared;
using Game339.Shared.DependencyInjection;

public class TurnEngine
{
    //===== Turn Events =====
    public event Action<ICharacter, ICharacter> EncounterSetup;
    public event Action EncounterStart;
    public event Action<bool> EncounterEnd;
    public event Action<int> PlayerTurnStart;
    public event Action PlayerTurnEnd;
    public event Action<int> EnemyTurnStart;
    public event Action EnemyTurnEnd;
    public event Action TurnStart;
    public event Action TurnEnd;

    //===== Global Information =====
    public ObservableValue<bool> IsPlayerTurn { get; } = new ObservableValue<bool>();
    public bool IsEncounterRunning { private set; get; } = false;
    public bool HasTurnStarted { private set; get; } = false;

    //===== Encounter Information =====
    public int TurnIndex { get; private set; } = 0;
    private ActionPair _firstTurns = new ActionPair();
    private ActionPair _secondTurns = new ActionPair();
    private ActionPair _currentTurns;

    private ICharacter _player;
    private ICharacter _enemy;

    private TurnState _currentState;
    public TurnState State
    {
        get => _currentState;
        set
        {
            // Debug.Log("switching to turn state: " + value);
            if (_player == null || _enemy == null)
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
                    TurnStart?.Invoke();
                    StartTurn();
                    break;
                case TurnState.EndTurn:
                    TurnEnd?.Invoke();
                    EndTurn();
                    break;
                case TurnState.ExitEncounter:
                    bool isPlayerWin = ExitEncounter();
                    EncounterEnd?.Invoke(isPlayerWin);
                    _player = _enemy = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            _currentState = value;
        }
    }

    public void SetupForNewEncounter(ICharacter player, ICharacter enemy)
    {
        TurnIndex = 0;
        IsPlayerTurn.Value = false;
        
        _player = player;
        _enemy = enemy;
        EncounterSetup?.Invoke(player, enemy);
    }
    
    //===== Encounter =====
    private void EnterEncounter()
    {
        IsEncounterRunning = true;
        
        //resolve who goes first
        if (_enemy.Speed.Value > _player.Speed.Value)
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
        IsEncounterRunning = false;

        _firstTurns.Clear();
        _secondTurns.Clear();
        
        return _player.HP.Value != 0;
    }

    //===== Turns =====
    private void StartTurn()
    {
        HasTurnStarted = true;
        
        TurnIndex += 1;
        _currentTurns.First?.Invoke();
    }

    private void EndTurn()
    {
        HasTurnStarted = false;
        
        _currentTurns.Second?.Invoke();
        
        //switch turn order
        _currentTurns = TurnIndex % 2 == 0 ? _firstTurns : _secondTurns;
    }
    
    //===== Character Turns =====
    //-- Player --
    private void StartPlayerTurn()
    {
        IsPlayerTurn.Value = true;
        PlayerTurnStart?.Invoke(TurnIndex);
    }
    
    private void EndPlayerTurn()
    {
        IsPlayerTurn.Value = false;
        PlayerTurnEnd?.Invoke();
    }
    
    //-- Enemy --
    private void StartEnemyTurn()
    {
        EnemyTurnStart?.Invoke(TurnIndex);
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
    ExitEncounter
}
