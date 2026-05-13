using Game339.Shared.DependencyInjection;
using TMPro;
using UnityEngine;

/// <summary>
/// Drives turn-related UI:
///  1. A turn counter (increments at the start of each player turn).
///  2. A status label that cycles through "Passive Phase" → "Player's Turn" → "Enemy's Turn".
///  3. A glow effect on whichever CharacterView is currently active (or off during passive phase).
/// </summary>
public class TurnIndicatorView : Controller
{
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _turnLabel;
    [SerializeField] private TextMeshProUGUI _turnCounterLabel;

    [Header("Character Views (for glow)")]
    [SerializeField] private CharacterView _playerView;
    [SerializeField] private CharacterView _enemyView;

    [Header("Passive Phase (optional — leave null to keep old turn-start behavior)")]
    [SerializeField] private PassivePhaseController _passivePhaseController;
    [SerializeField] private string _passivePhaseText = "Passive Phase";

    [Header("Text Formatting")]
    [SerializeField] private string _playerTurnText = "Player's Turn";
    [SerializeField] private string _enemyTurnText = "Enemy's Turn";
    [Tooltip("{0} is replaced with the current turn number.")]
    [SerializeField] private string _turnCounterFormat = "Turn {0}";

    private int _turnNumber;

    protected override void Subscribe()
    {
        _turnEngine.EncounterSetup += OnEncounterSetup;
        _turnEngine.PlayerTurnStart += OnPlayerTurnStart;
        _turnEngine.PlayerTurnEnd += OnPlayerTurnEnd;
        _turnEngine.EncounterEnd += OnEncounterEnd;

        if (_passivePhaseController != null)
        {
            _passivePhaseController.OnPassivePhaseStart += OnPassivePhaseStart;
            _passivePhaseController.OnPassivePhaseComplete += OnPassivePhaseComplete;
        }

        ClearAll();
    }

    protected override void Unsubscribe()
    {
        _turnEngine.EncounterSetup -= OnEncounterSetup;
        _turnEngine.PlayerTurnStart -= OnPlayerTurnStart;
        _turnEngine.PlayerTurnEnd -= OnPlayerTurnEnd;
        _turnEngine.EncounterEnd -= OnEncounterEnd;

        if (_passivePhaseController != null)
        {
            _passivePhaseController.OnPassivePhaseStart -= OnPassivePhaseStart;
            _passivePhaseController.OnPassivePhaseComplete -= OnPassivePhaseComplete;
        }
    }

    //===== Event handlers =====

    private void OnEncounterSetup(ICharacter player, ICharacter enemy)
    {
        _turnNumber = 0;
        ClearAll();
    }

    private void OnPlayerTurnStart()
    {
        _turnNumber++;
        UpdateTurnCounter();

        // If no PassivePhaseController is wired, fall back to old behavior: flip to player's turn immediately.
        // Otherwise wait for OnPassivePhaseStart / OnPassivePhaseComplete.
        if (_passivePhaseController == null)
        {
            SetTurnLabel(_playerTurnText);
            SetPlayerGlow(true);
            SetEnemyGlow(false);
        }
    }

    private void OnPassivePhaseStart()
    {
        SetTurnLabel(_passivePhaseText);
        SetPlayerGlow(false);
        SetEnemyGlow(false);
    }

    private void OnPassivePhaseComplete()
    {
        SetTurnLabel(_playerTurnText);
        SetPlayerGlow(true);
        SetEnemyGlow(false);
    }

    private void OnPlayerTurnEnd()
    {
        SetTurnLabel(_enemyTurnText);
        SetPlayerGlow(false);
        SetEnemyGlow(true);
    }

    private void OnEncounterEnd(bool _)
    {
        ClearAll();
    }

    //===== Helpers =====

    private void ClearAll()
    {
        SetTurnLabel("");
        SetPlayerGlow(false);
        SetEnemyGlow(false);
        UpdateTurnCounter();
    }

    private void UpdateTurnCounter()
    {
        if (_turnCounterLabel == null) return;
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