using Game.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private Image _characterImage;
    
    [Header("View")]
    [SerializeField] private HealthView _healthView;

    private Character _character;
    
    public void Initialize(Character character)
    {
        _character = character;
        _characterImage.sprite = character.Icon;
        _healthView.Subscribe(character);
    }

    private void OnDestroy()
    {
        _healthView.Unsubscribe();
    }
}
