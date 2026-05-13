using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent panel for the upgrade screen. Holds references to all sub-components, routes their
/// events into UpgradeController, and exposes display methods the controller calls.
/// </summary>
public class UpgradeView : TypedView<UpgradeController>, IGamePanel
{
    [SerializeField] private CanvasGroup _panel;

    [Header("Sub-components")]
    [SerializeField] private OfferedLimbUI _offeredArmUI;
    [SerializeField] private OfferedLimbUI _offeredLegUI;
    [SerializeField] private CreaturePreviewUI _creaturePreviewUI;
    [SerializeField] private UpgradeConfirmationUI _confirmationUI;
    [SerializeField] private StatsPreviewView _statsPreviewView;

    private UpgradeController _controller;

    private void Start()
    {
        SetVisible(false);
    }

    public void SetVisible(bool value)
    {
        if (_panel == null) return;
        _panel.alpha = value ? 1f : 0f;
        _panel.interactable = value;
        _panel.blocksRaycasts = value;
    }

    protected override void InitializeView(UpgradeController[] args)
    {
        _controller = args[0];

        _offeredArmUI.OnClicked += HandleOfferClicked;
        _offeredArmUI.OnHoverEnter += HandleOfferHoverEnter;
        _offeredArmUI.OnHoverExit += HandleOfferHoverExit;

        _offeredLegUI.OnClicked += HandleOfferClicked;
        _offeredLegUI.OnHoverEnter += HandleOfferHoverEnter;
        _offeredLegUI.OnHoverExit += HandleOfferHoverExit;

        _creaturePreviewUI.OnSlotClicked += HandleSlotClicked;

        _confirmationUI.OnConfirm += HandleConfirm;
        _confirmationUI.OnBack += HandleBack;

        _controller.IsUpgradeAvailable.ChangeEvent += SetVisible;

        _statsPreviewView.Initialize(_controller.GetPlayer());
    }

    protected override void DeinitializeView()
    {
        _offeredArmUI.OnClicked -= HandleOfferClicked;
        _offeredArmUI.OnHoverEnter -= HandleOfferHoverEnter;
        _offeredArmUI.OnHoverExit -= HandleOfferHoverExit;

        _offeredLegUI.OnClicked -= HandleOfferClicked;
        _offeredLegUI.OnHoverEnter -= HandleOfferHoverEnter;
        _offeredLegUI.OnHoverExit -= HandleOfferHoverExit;

        _creaturePreviewUI.OnSlotClicked -= HandleSlotClicked;

        _confirmationUI.OnConfirm -= HandleConfirm;
        _confirmationUI.OnBack -= HandleBack;

        _controller.IsUpgradeAvailable.ChangeEvent -= SetVisible;

        _statsPreviewView.Deinitialize();
        _controller = null;
    }

    //===== Display methods (called by controller) =====

    public void DisplayOffers(BodyPart armOffer, BodyPart legOffer, Character owner)
    {
        _offeredArmUI.Populate(armOffer, owner);
        _offeredLegUI.Populate(legOffer, owner);
    }

    public void PopulateCreaturePreview(Character player)
    {
        _creaturePreviewUI.Populate(player);
    }

    public void EnableSlotsForType(BodyPartType type, bool enabled)
    {
        _creaturePreviewUI.SetSlotsInteractable(type, enabled);
    }

    public void DisableAllSlots()
    {
        _creaturePreviewUI.DisableAllSlots();
    }

    public void ShowConfirmation(BodyPart oldPart, BodyPart newPart, Character owner)
    {
        _confirmationUI.Show(oldPart, newPart, owner);
    }

    public void HideConfirmation()
    {
        _confirmationUI.Hide();
    }

    public void UpdateStatsPreview(int previewMaxHP, int previewAttack)
    {
        _statsPreviewView.SetPreview(previewMaxHP, previewAttack);
    }

    public void ClearStatsPreview()
    {
        _statsPreviewView.ClearPreview();
    }

    //===== Sub-component event forwarders =====

    private void HandleOfferClicked(OfferedLimbUI offer) => _controller.OnOfferClicked(offer.Part);
    private void HandleOfferHoverEnter(OfferedLimbUI offer) => _controller.OnOfferHoverEnter(offer.Part);
    private void HandleOfferHoverExit(OfferedLimbUI offer) => _controller.OnOfferHoverExit();
    private void HandleSlotClicked(BodyPartType type, int index) => _controller.OnSlotClicked(type, index);
    private void HandleConfirm() => _controller.OnConfirm();
    private void HandleBack() => _controller.OnBack();
}