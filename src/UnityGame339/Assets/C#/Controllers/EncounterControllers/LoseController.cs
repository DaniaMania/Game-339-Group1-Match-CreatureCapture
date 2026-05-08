using Game339.Shared;
using UnityEngine;

public class LoseController : GameController
{
    public ObservableValue<bool> IsLoseShowing { get; } = new ObservableValue<bool>();

    protected new void Start()
    {
        base.Start();
        IsLoseShowing.Value = false;
    }

    protected override void Subscribe()
    {
        _turnEngine.EncounterStart += DisableLose;
        _turnEngine.EncounterEnd += TestLose;
    }
    
    protected override void Unsubscribe()
    {
        _turnEngine.EncounterStart -= DisableLose;
        _turnEngine.EncounterEnd -= TestLose;
    }

    private void DisableLose()
    {
        IsLoseShowing.Value = false;
    }

    private void TestLose(bool isPlayerWin)
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