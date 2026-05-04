using Game.Runtime;
using UnityEngine;

public class BattleView : ObserverMonoBehaviour
{
    [SerializeField] private CharacterView _playerView;
    [SerializeField] private CharacterView _enemyView;
    
    protected override void Subscribe()
    {
        ServiceResolver.Resolve<TurnEngine>().EncounterSetup += InitializeViews;
    }

    protected override void Unsubscribe()
    {
        ServiceResolver.Resolve<TurnEngine>().EncounterSetup -= InitializeViews;
    }

    private void InitializeViews(Character player, Character enemy)
    {
        _playerView.Initialize(player);
        _enemyView.Initialize(enemy);
    }
}
