using Game.Runtime;
using Game339.Shared.Diagnostics;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField] private GameObject _titlePanel;

    private void Start()
    {
        _titlePanel.SetActive(true);
    }

    public void OnPlayClicked()
    {
        ServiceResolver.Resolve<IGameLog>().Info("[Title] Play Button Clicked");
        _titlePanel.SetActive(false);
        EncounterManager.Instance.BeginNewEncounter();
    }
}