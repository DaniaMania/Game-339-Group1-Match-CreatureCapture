using UnityEngine;
using UnityEngine.UI;

public class PlayerBodyDisplay : TypedView<Character>
{
    [Header("Head / Torso (display only)")]
    [SerializeField] private Image _headImage;

    [Header("Arm Slots")]
    [SerializeField] private BodyPartSlotUI _leftArmSlot;
    [SerializeField] private BodyPartSlotUI _rightArmSlot;

    [Header("Leg Slots")]
    [SerializeField] private BodyPartSlotUI _leftLegSlot;
    [SerializeField] private BodyPartSlotUI _rightLegSlot;

    private Character _character;

    protected override void InitializeView(Character[] character)
    {
        _character = character[0];

        if (_headImage != null && _character.Icon != null)
            _headImage.sprite = _character.Icon;

        RefreshSlots();
    }

    public void RefreshSlots()
    {
        CreatureLoadout loadout = _character.Loadout;

        _leftArmSlot.Populate(loadout.arms.Length > 0 ? loadout.arms[0] : null);
        _rightArmSlot.Populate(loadout.arms.Length > 1 ? loadout.arms[1] : null);
        _leftLegSlot.Populate(loadout.legs.Length > 0 ? loadout.legs[0] : null);
        _rightLegSlot.Populate(loadout.legs.Length > 1 ? loadout.legs[1] : null);
    }

    protected override void DeinitializeView()
    {
        _leftArmSlot.Populate(null);
        _rightArmSlot.Populate(null);
        _leftLegSlot.Populate(null);
        _rightLegSlot.Populate(null);
        _character = null;
    }
}