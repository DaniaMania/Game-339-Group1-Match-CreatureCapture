using Game.Runtime;
using UnityEngine;

public class BattleView : MonoBehaviour, IGamePanel
{
    [SerializeField] private CharacterView _playerView;
    [SerializeField] private CharacterView _enemyView;

    private TurnEngine _turnEngine;

    private void Awake()
    {
        _turnEngine = ServiceResolver.Resolve<TurnEngine>();
        _turnEngine.EncounterStart += OnEncounterStart;
        _turnEngine.EncounterEnd += OnEncounterEnd;
    }

    private void OnDestroy()
    {
        if (_turnEngine == null) return;
        _turnEngine.EncounterStart -= OnEncounterStart;
        _turnEngine.EncounterEnd -= OnEncounterEnd;
    }

    public void SetVisible(bool visible) => gameObject.SetActive(visible);
    private void OnEncounterStart() => SetVisible(true);
    private void OnEncounterEnd(bool playerWon) => SetVisible(false);
}