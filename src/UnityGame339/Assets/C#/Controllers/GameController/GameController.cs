using Game.Runtime;
using UnityEngine;

/// <summary>
/// change/update views over the course of the entire game  
/// </summary>
public abstract class GameController : Controller
{
    protected Character Player => CharacterDatabase.PlayerCharacter;
}
