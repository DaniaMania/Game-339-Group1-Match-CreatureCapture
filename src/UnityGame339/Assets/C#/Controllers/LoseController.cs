using Game339.Shared;
using UnityEngine;

public class LoseController : Controller
{
    public ObservableValue<bool> IsLoseShowing { private set; get; } = new ObservableValue<bool>();

    protected override void Subscribe()
    {
        _turnEngine.EncounterEnd += OnEncounterEnd;
    }

    protected override void Unsubscribe()
    {
        _turnEngine.EncounterEnd -= OnEncounterEnd;
    }
    
    private void OnEncounterEnd(bool playerWon)
    {
        if (playerWon) return;
        IsLoseShowing.Value = true;
    }
    
    // called by LoseView button
    public void Restart()
    {
        if (!IsLoseShowing.Value) return;
        Player.ResetValues();
        Enemy.ResetValues();
        IsLoseShowing.Value = false;
        
        EncounterManager.Instance.BeginNewEncounter();
    }
    
    // called by LoseView button
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}