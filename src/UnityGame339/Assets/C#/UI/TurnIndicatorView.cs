using Game.Runtime;
using Game339.Shared.DependencyInjection;
using TMPro;
using UnityEngine;

/// <summary>
/// Drives three pieces of turn-related UI:
///  1. A turn counter (increments at the start of each player turn).
///  2. A "Player's Turn" / "Enemy's Turn" label at the top.
///  3. A glow effect on whichever CharacterView is currently active.
/// Inherits from Controller so it gets the standard subscribe lifecycle and a TurnEngine reference.
/// </summary>
public class TurnIndicatorView : TypedView<CharacterView>
{
    private readonly TurnEngine _turnEngine = ServiceResolver.Resolve<TurnEngine>();
    
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _turnLabel;
    [SerializeField] private TextMeshProUGUI _turnCounterLabel;
    
    private CharacterView _playerView;
    private CharacterView _enemyView;

    [Header("Text Formatting")]
    [SerializeField] private string _playerTurnText = "Player's Turn";
    [SerializeField] private string _enemyTurnText = "Enemy's Turn";
    [Tooltip("{0} is replaced with the current turn number.")]
    [SerializeField] private string _turnCounterFormat = "Turn {0}";

    private int _turnNumber;
    
    protected override void InitializeView(CharacterView[] arg)
    {
        _playerView = arg[0];
        _enemyView = arg[1];
        
        _turnEngine.EncounterSetup += OnEncounterSetup;
        _turnEngine.PlayerTurnStart += OnPlayerTurnStart;
        _turnEngine.PlayerTurnEnd += OnPlayerTurnEnd;
        _turnEngine.EncounterEnd += OnEncounterEnd;
        
        // The view may be enabled mid-game (e.g. enabled in the editor after Awake).
        // Default state: clear labels and no glow until the next encounter starts.
        ClearAll();
    }

    protected override void DeinitializeView()
    {
        _playerView = _enemyView = null;
        
        _turnEngine.EncounterSetup -= OnEncounterSetup;
        _turnEngine.PlayerTurnStart -= OnPlayerTurnStart;
        _turnEngine.PlayerTurnEnd -= OnPlayerTurnEnd;
        _turnEngine.EncounterEnd -= OnEncounterEnd;
    }

    //===== Event handlers =====

    private void OnEncounterSetup(ICharacter player, ICharacter enemy)
    {
        // Fresh encounter — the next PlayerTurnStart will be Turn 1.
        _turnNumber = 0;
        UpdateTurnCounter();
        SetTurnLabel("");
        SetPlayerGlow(false);
        SetEnemyGlow(false);
    }

    private void OnPlayerTurnStart()
    {
        _turnNumber++;
        UpdateTurnCounter();
        SetTurnLabel(_playerTurnText);
        SetPlayerGlow(true);
        SetEnemyGlow(false);
    }

    /// <summary>
    /// PlayerTurnEnd fires the instant before the enemy turn begins, so we use it as the
    /// "enemy's turn starting" cue. (No separate EnemyTurnStart subscription needed — and
    /// this works whether or not the engine exposes such an event.)
    /// </summary>
    private void OnPlayerTurnEnd()
    {
        SetTurnLabel(_enemyTurnText);
        SetPlayerGlow(false);
        SetEnemyGlow(true);
    }

    private void OnEncounterEnd(bool _)
    {
        SetTurnLabel("");
        SetPlayerGlow(false);
        SetEnemyGlow(false);
    }

    //===== Helpers =====

    private void ClearAll()
    {
        _turnNumber = 0;
        UpdateTurnCounter();
        SetTurnLabel("");
        SetPlayerGlow(false);
        SetEnemyGlow(false);
    }

    private void UpdateTurnCounter()
    {
        if (_turnCounterLabel == null) return;
        // Show empty during pre-encounter state (turn 0), otherwise show formatted count.
        _turnCounterLabel.text = _turnNumber > 0
            ? string.Format(_turnCounterFormat, _turnNumber)
            : "";
    }

    private void SetTurnLabel(string text)
    {
        if (_turnLabel != null) _turnLabel.text = text;
    }

    private void SetPlayerGlow(bool active)
    {
        if (_playerView != null) _playerView.SetGlow(active);
    }

    private void SetEnemyGlow(bool active)
    {
        if (_enemyView != null) _enemyView.SetGlow(active);
    }
}
