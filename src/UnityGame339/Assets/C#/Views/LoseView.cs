using UnityEngine;
using UnityEngine.UI;

public class LoseView : MonoBehaviour, IGamePanel
{
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private LoseController _controller;

    private void Awake()
    {
        _controller.IsLoseShowing.ChangeEvent += SetVisible;
    }

    private void OnDestroy()
    {
        if (_controller != null) _controller.IsLoseShowing.ChangeEvent -= SetVisible;
    }

    private void Start()
    {
        _restartButton.onClick.AddListener(_controller.Restart);
        _quitButton.onClick.AddListener(_controller.Quit);
        SetVisible(false);
    }

    public void SetVisible(bool value)
    {
        _panel.alpha = (value) ? 1f : 0f;
        _panel.interactable = value;
        _panel.blocksRaycasts = value;
    }
}
