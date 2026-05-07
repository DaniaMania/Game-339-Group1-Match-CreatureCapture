using System;
using UnityEngine;

public class EnemyWrapper
{
    public event Action<int> WrapperOnTakeDamage;
    public event Action WrapperOnDeath;
    
    public Character Enemy { get; private set; }

    public void SetupForNewEnemy(Character enemy)
    {
        Enemy = enemy;

        Enemy.OnCharacterTakeDamage += WrapperOnTakeDamage;
        Enemy.OnCharacterDeath += WrapperOnDeath;
    }

    public void CleanEncounter()
    {
        Enemy.OnCharacterTakeDamage -= WrapperOnTakeDamage;
        Enemy.OnCharacterDeath -= WrapperOnDeath;

        Enemy = null;
    }
}
