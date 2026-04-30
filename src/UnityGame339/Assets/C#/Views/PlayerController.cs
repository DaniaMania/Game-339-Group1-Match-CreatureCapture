using System;
using Game.Runtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private PlayerData _playerData;

    private Character _player;
    private Character _enemy;
    private AttackService _attackService;
    private TurnEngine _turnEngine;

    private int _currentHealAmount;

    public event Action<int> PlayerTakeDamage;
    public event Action<int> EnemyTakeDamage;
    public event Action PlayerHeal;
    public event Action EnemyHeal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _player = ServiceResolver.Resolve<CharacterManager>().Get(_playerData.PlayerCharacter);
        _enemy = ServiceResolver.Resolve<CharacterManager>().Get(_playerData.EnemyCharacter);
        _attackService = ServiceResolver.Resolve<AttackService>();
        _turnEngine = ServiceResolver.Resolve<TurnEngine>();

        _player.ResetValues();
        _enemy.ResetValues();

        _currentHealAmount = _playerData.HealAmount;
    }

    private void Start()
    {
        _turnEngine.EnterEncounter();
        _turnEngine.StartPlayerTurn();
    }

    //===== Character Abilities =====
    public void PlayerAttack()
    {
        if (Attack(_player, _enemy)) _turnEngine.EndPlayerTurn();
    }

    public void EnemyAttack()
    {
        if (Attack(_enemy, _player)) _turnEngine.StartPlayerTurn();
    }

    private bool Attack(Character attacker, Character target)
    {
        bool isTargetDead = _attackService.Attack(attacker, target);
        if (isTargetDead)
        {
            bool isPlayerAttacker = attacker == _player;
            _turnEngine.ExitEncounter(isPlayerAttacker);
        }
        return !isTargetDead;
    }

    public void Heal()
    {
        int healed = Mathf.Min(_player.HP.Value + _currentHealAmount, _player.MaxHP.Value);
        _player.HP.Value = healed;
        _turnEngine.EndPlayerTurn();
    }

    //===== Upgrades =====
    public void UpgradeHealPotency(int amount)
    {
        _currentHealAmount += amount;
    }

    public void ResetHealPotency()
    {
        _currentHealAmount = _playerData.HealAmount;
    }
}