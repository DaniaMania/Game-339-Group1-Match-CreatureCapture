using Game339.Shared.DependencyInjection;
using UnityEngine;

/// <summary>
/// The persistent left panel. Owns the stats display and the player preview.
/// Lives outside the battle / upgrade view hierarchy so it stays visible across phases.
///
/// Currently shows: stats (HP, Attack) + player preview (hoverable limbs).
/// Planned additions: encounter counter, lives display, etc.
///
/// UpgradeView calls SetPlayerPreviewVisible(false) while the upgrade screen is open
/// (the upgrade screen has its own creature preview in the middle, so a duplicate on the
/// side would be redundant).
/// </summary>
public class SidePanelView : GameController
{
    [SerializeField] private StatsPreviewView _statsPreviewView;
    [SerializeField] private PlayerPreviewUI _playerPreviewUI;

    protected override void Subscribe()
    {
        // Refresh the limb preview at the start of each encounter so post-upgrade loadout
        // changes are reflected. Stats track Player.MaxHP/Attack via ChangeEvent so they
        // stay current automatically — no refresh hook needed for the stats half.
        _turnEngine.EncounterSetup += OnEncounterSetup;

        _statsPreviewView.Initialize(Player);
        _playerPreviewUI.Initialize(Player);
    }

    protected override void Unsubscribe()
    {
        _turnEngine.EncounterSetup -= OnEncounterSetup;
        _statsPreviewView.Deinitialize();
        _playerPreviewUI.Deinitialize();
    }

    private void OnEncounterSetup(ICharacter player, ICharacter enemy)
    {
        _playerPreviewUI.RefreshLimbs();
    }

    //===== Public API =====

    public void SetPlayerPreviewVisible(bool visible)
    {
        if (_playerPreviewUI != null) _playerPreviewUI.gameObject.SetActive(visible);
    }

    public void SetStatsPreview(int maxHP, int attack)
    {
        if (_statsPreviewView != null) _statsPreviewView.SetPreview(maxHP, attack);
    }

    public void ClearStatsPreview()
    {
        if (_statsPreviewView != null) _statsPreviewView.ClearPreview();
    }
}
