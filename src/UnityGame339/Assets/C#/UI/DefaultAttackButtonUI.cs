using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to the default attack button GameObject. Shows the shared tooltip on hover
/// with the player's current Attack value. The PlayerControllerView wires up the owner.
/// </summary>
public class DefaultAttackButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string _attackName = "Basic Attack";

    [TextArea]
    [SerializeField] private string _attackDescription = "A standard attack using your current Attack stat. Always available with no cooldown.";

    [Tooltip("Optional. If unset, the tooltip's icon area hides.")]
    [SerializeField] private Sprite _attackIcon;

    private Character _owner;

    public void SetOwner(Character owner)
    {
        _owner = owner;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_owner == null || TooltipUI.Instance == null) return;

        int damage = _owner.Attack.Value;
        string effect = $"Deal {damage} damage";

        TooltipUI.Instance.Show(
            _attackName,
            effect,
            _attackDescription,
            (RectTransform)transform,
            cooldownInfo: null,
            icon: _attackIcon);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance == null) return;
        TooltipUI.Instance.Hide();
    }
}