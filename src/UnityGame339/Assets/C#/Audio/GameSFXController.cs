using Game.Runtime;
using Game339.Shared.DependencyInjection;
using UnityEngine;

public class GameSFXController : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private AudioClip _playerAttackClip;
    [SerializeField] private AudioClip _playerTakeDamageClip;
    [SerializeField] private AudioClip _playerBlockClip;
    [SerializeField] private AudioClip _playerHealClip;
    [SerializeField] private AudioClip _enemyDeathClip;
    [SerializeField] private AudioClip _playerDeathClip;

    [Header("Turn")]
    [SerializeField] private AudioClip _playerTurnStartClip;
    [SerializeField] private AudioClip _enemyTurnStartClip;

    [Header("Encounter")]
    [SerializeField] private AudioClip _encounterStartClip;
    [SerializeField] private AudioClip _encounterWinClip;
    [SerializeField] private AudioClip _encounterLoseClip;

    private AudioService _audioService;
    private TurnEngine   _turnEngine;
    private Character    _player;
    private Character    _enemy;
    private int          _lastPlayerHP;
    private int          _lastPlayerBlock;

    private void Start()
    {
        _audioService = ServiceResolver.Resolve<AudioService>();
        _turnEngine   = ServiceResolver.Resolve<TurnEngine>();

        _turnEngine.EncounterSetup  += OnEncounterSetup;
        _turnEngine.EncounterStart  += OnEncounterStart;
        _turnEngine.EncounterEnd    += OnEncounterEnd;
        _turnEngine.PlayerTurnStart += OnPlayerTurnStart;
        _turnEngine.EnemyTurnStart  += OnEnemyTurnStart;
    }

    private void OnDestroy()
    {
        if (_turnEngine != null)
        {
            _turnEngine.EncounterSetup  -= OnEncounterSetup;
            _turnEngine.EncounterStart  -= OnEncounterStart;
            _turnEngine.EncounterEnd    -= OnEncounterEnd;
            _turnEngine.PlayerTurnStart -= OnPlayerTurnStart;
            _turnEngine.EnemyTurnStart  -= OnEnemyTurnStart;
        }

        UnsubscribeCharacters();
    }

    private void OnEncounterSetup(ICharacter player, ICharacter enemy)
    {
        UnsubscribeCharacters();

        _player = (Character)player;
        _enemy  = (Character)enemy;

        _lastPlayerHP    = _player.HP.Value;
        _lastPlayerBlock = _player.Block.Value;

        _player.HP.ChangeEvent        += OnPlayerHPChanged;
        _player.Block.ChangeEvent     += OnPlayerBlockChanged;
        _player.OnCharacterTakeDamage += OnPlayerTakeDamage;
        _player.OnCharacterDeath      += OnPlayerDeath;
        _enemy.OnCharacterTakeDamage  += OnEnemyTakeDamage;
        _enemy.OnCharacterDeath       += OnEnemyDeath;
    }

    private void UnsubscribeCharacters()
    {
        if (_player != null)
        {
            _player.HP.ChangeEvent        -= OnPlayerHPChanged;
            _player.Block.ChangeEvent     -= OnPlayerBlockChanged;
            _player.OnCharacterTakeDamage -= OnPlayerTakeDamage;
            _player.OnCharacterDeath      -= OnPlayerDeath;
        }
        if (_enemy != null)
        {
            _enemy.OnCharacterTakeDamage -= OnEnemyTakeDamage;
            _enemy.OnCharacterDeath      -= OnEnemyDeath;
        }
    }

    private void Play(AudioClip clip)
    {
        if (clip != null) _audioService.PlaySFX(clip);
    }

    private void OnPlayerHPChanged(int newHP)
    {
        if (newHP > _lastPlayerHP) Play(_playerHealClip);
        _lastPlayerHP = newHP;
    }

    private void OnPlayerBlockChanged(int newBlock)
    {
        if (newBlock > _lastPlayerBlock) Play(_playerBlockClip);
        _lastPlayerBlock = newBlock;
    }

    private void OnEncounterStart()             => Play(_encounterStartClip);
    private void OnEncounterEnd(bool playerWin) => Play(playerWin ? _encounterWinClip : _encounterLoseClip);
    private void OnPlayerTurnStart()            => Play(_playerTurnStartClip);
    private void OnEnemyTurnStart()             => Play(_enemyTurnStartClip);
    private void OnPlayerTakeDamage(int _)      => Play(_playerTakeDamageClip);
    private void OnEnemyTakeDamage(int _)       => Play(_playerAttackClip);
    private void OnPlayerDeath()                => Play(_playerDeathClip);
    private void OnEnemyDeath()                 => Play(_enemyDeathClip);
}