using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerView : TypedView<PlayerController>
{
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _healButton;

    private PlayerController _playerController;

    private void Start()
    {
        SetInteractable(false);
    }

    protected override void InitializeView(PlayerController[] arg)
    {
        _playerController = arg[0];
        
        _attackButton.onClick.AddListener(_playerController.Attack);
        _healButton.onClick.AddListener(_playerController.Heal);
        
        _playerController.IsInteractable.ChangeEvent += SetInteractable;
    }

    protected override void DeinitializeView()
    {
        _attackButton.onClick.RemoveListener(_playerController.Attack);
        _healButton.onClick.RemoveListener(_playerController.Heal);
        
        _playerController.IsInteractable.ChangeEvent -= SetInteractable;
    }
    
    private void SetInteractable(bool value)
    {
        _attackButton.interactable = value;
        _healButton.interactable = value;
    }
}
