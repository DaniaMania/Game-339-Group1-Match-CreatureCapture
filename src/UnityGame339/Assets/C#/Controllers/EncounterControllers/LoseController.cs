using Game339.Shared;
using UnityEngine;

public class LoseController : EncounterController
{
    public ObservableValue<bool> IsLoseShowing { get; } = new ObservableValue<bool>();

    protected override void EncounterBegin()
    {
        IsLoseShowing.Value = false;
    }

    protected override void EncounterEnd(bool isPlayerWin)
    {
        if (isPlayerWin) return;
        IsLoseShowing.Value = true;
    }
    
    // called by LoseView button
    public void Restart()
    {
        if (!IsLoseShowing.Value) return;
        Player.ResetValues();
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