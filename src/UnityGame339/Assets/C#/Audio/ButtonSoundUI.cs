using Game.Runtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundUI : MonoBehaviour
{
    [SerializeField] private AudioClip _clickClip;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_clickClip != null)
            ServiceResolver.Resolve<AudioService>().PlaySFX(_clickClip);
    }
}