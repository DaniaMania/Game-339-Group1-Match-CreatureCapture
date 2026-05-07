using Game.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : TypedView<Character>
{
    [Header("Values")]
    [SerializeField] private Image _characterImage;
    
    [Header("View")]
    [SerializeField] private HealthView _healthView;

    private Character _character;
    
    protected override void InitializeView(Character[] character)
    {
        _character = character[0];
        _characterImage.sprite = _character.Icon;
        _healthView.Initialize(character);
    }

    protected override void DeinitializeView()
    {
        _character = null;
        if (_characterImage) _characterImage.sprite = null;
        _healthView.Deinitialize();
    }
}
