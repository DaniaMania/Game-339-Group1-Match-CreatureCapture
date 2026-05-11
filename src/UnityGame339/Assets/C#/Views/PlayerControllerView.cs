using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerView : TypedView<PlayerController>
{
    [SerializeField] private Button _defaultAttackButton;
    [SerializeField] private DefaultAttackButtonUI _defaultAttackTooltipTrigger;
    [SerializeField] private ActionButtonUI _arm1Button;
    [SerializeField] private ActionButtonUI _arm2Button;

    private PlayerController _playerController;

    private void Start()
    {
        SetInteractable(false);
    }

    protected override void InitializeView(PlayerController[] arg)
    {
        _playerController = arg[0];

        _defaultAttackButton.onClick.AddListener(_playerController.DefaultAttack);
        _arm1Button.OnActionChosen += OnArm1Chosen;
        _arm2Button.OnActionChosen += OnArm2Chosen;

        _playerController.IsInteractable.ChangeEvent += SetInteractable;
    }

    protected override void DeinitializeView()
    {
        _defaultAttackButton.onClick.RemoveListener(_playerController.DefaultAttack);
        _arm1Button.OnActionChosen -= OnArm1Chosen;
        _arm2Button.OnActionChosen -= OnArm2Chosen;

        _playerController.IsInteractable.ChangeEvent -= SetInteractable;
    }

    public void PopulateArms(BodyPart[] arms, Character owner)
    {
        BodyPart arm1 = (arms != null && arms.Length > 0) ? arms[0] : null;
        BodyPart arm2 = (arms != null && arms.Length > 1) ? arms[1] : null;
        _arm1Button.Populate(arm1, owner);
        _arm2Button.Populate(arm2, owner);

        if (_defaultAttackTooltipTrigger != null) _defaultAttackTooltipTrigger.SetOwner(owner);
    }

    public void RefreshArmCooldowns(int[] cooldowns)
    {
        if (cooldowns == null) return;
        if (cooldowns.Length > 0) _arm1Button.SetCooldown(cooldowns[0]);
        if (cooldowns.Length > 1) _arm2Button.SetCooldown(cooldowns[1]);
    }

    private void OnArm1Chosen(BodyPart _) => _playerController.UseArm(0);
    private void OnArm2Chosen(BodyPart _) => _playerController.UseArm(1);

    private void SetInteractable(bool value)
    {
        _defaultAttackButton.interactable = value;
        _arm1Button.SetInteractable(value);
        _arm2Button.SetInteractable(value);
    }
}