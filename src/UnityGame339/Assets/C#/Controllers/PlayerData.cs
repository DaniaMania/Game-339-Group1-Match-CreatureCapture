using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] public string PlayerCharacter;
    [SerializeField] public string EnemyCharacter;
    [SerializeField] public int HealAmount = 2;
}