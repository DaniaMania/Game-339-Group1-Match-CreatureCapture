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
        Debug.Log("[Title] OnPlayClicked");
        _titlePanel.SetActive(false);
        EncounterManager.Instance.BeginNewEncounter();
    }
}