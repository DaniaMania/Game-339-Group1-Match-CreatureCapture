using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : TypedView<Character>
{
    [Header("Initialize")]
    [SerializeField] private Character _character;

    [Header("Values")]
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _nameText;

    [Header("View")]
    [SerializeField] private HealthView _healthView;

    private void Awake()
    {
        if (_character != null)
            Initialize(_character);
    }

    protected override void InitializeView(Character[] character)
    {
        var c = character[0];
        if (_characterImage) _characterImage.sprite = c.Icon;
        if (_nameText) _nameText.text = c.name;
        _healthView.Initialize(c);
    }

    protected override void DeinitializeView()
    {
        if (_characterImage) _characterImage.sprite = null;
        _healthView.Deinitialize();
    }
}