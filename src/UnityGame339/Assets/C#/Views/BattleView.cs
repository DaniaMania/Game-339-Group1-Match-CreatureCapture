using Game.Runtime;
using UnityEngine;

public class BattleView : EncounterController
{
    [SerializeField] private TurnIndicatorView _turnIndicatorView;
    [SerializeField] private CharacterView _playerView;
    [SerializeField] private CharacterView _enemyView;

    protected override void EncounterBegin()
    {
        _playerView.Initialize(Player);
        _enemyView.Initialize(Enemy);
        _turnIndicatorView.Initialize(_playerView, _enemyView);
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        _playerView.Deinitialize();
        _enemyView.Deinitialize();
        _turnIndicatorView.Deinitialize();  
    }
}