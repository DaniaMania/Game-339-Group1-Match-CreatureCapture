using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : TypedView<UpgradeController>, IGamePanel
{
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private Button _upgradeAttackButton;
    [SerializeField] private Button _healButton;
    [SerializeField] private Button _upgradeHealButton;
    
    private UpgradeController _upgradeController;

    private void Start()
    {
        SetVisible(false);
    }

    public void SetVisible(bool value)
    {
        _panel.alpha = (value) ? 1f : 0f;
        _panel.interactable = value;
        _panel.blocksRaycasts = value;
    }

    protected override void InitializeView(UpgradeController[] args)
    {
        _upgradeController = args[0];
        
        _upgradeAttackButton.onClick.AddListener(_upgradeController.UpgradeAttack);
        _healButton.onClick.AddListener(_upgradeController.HealToFull);
        _upgradeHealButton.onClick.AddListener(_upgradeController.UpgradeHealPotency);
        
        _upgradeController.IsUpgradeAvailable.ChangeEvent += SetVisible;
    }

    protected override void DeinitializeView()
    {
        _upgradeController.IsUpgradeAvailable.ChangeEvent -= SetVisible;
    }
}