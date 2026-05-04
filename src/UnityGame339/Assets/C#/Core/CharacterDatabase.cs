using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        //reset characters
        ResetPlayerValues();
    }

    [SerializeField] private Character _player;
    [SerializeField] private Character[] _enemyList;

    //===== Static References =====
    
    public static Character PlayerCharacter => Instance._player;

    public static Character GetEnemyCharacterFromIndex(int index) => Instance._enemyList[index % Instance._enemyList.Length];
    public static Character GetRandomCharacter() => Instance._enemyList.RandomEntry();
    
    //===== Instance References =====
    public void ResetPlayerValues()
    {
        _player.ResetValues();
    }
}

public static class ArrayExtension
{
    public static T RandomEntry<T>(this T[] target)
    {
        return target[Random.Range(0, target.Length)];
    }
}
