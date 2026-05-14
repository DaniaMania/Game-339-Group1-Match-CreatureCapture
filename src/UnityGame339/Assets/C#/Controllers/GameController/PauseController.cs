using DG.Tweening;
using Game.Runtime;
using Game339.Shared.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _quitToTitleButton;

    private TurnEngine _turnEngine;
    private IGameLog _logger;
    private bool _isPaused;

    private void Start()
    {
        _turnEngine = ServiceResolver.Resolve<TurnEngine>();
        _logger = ServiceResolver.Resolve<IGameLog>();
        
        _pausePanel.SetActive(false);
        _resumeButton.onClick.AddListener(Resume);
        _quitToTitleButton.onClick.AddListener(QuitToTitle);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void TogglePause()
    {
        if (_isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        _logger.Info("[Pause] Game Paused");
        
        _isPaused = true;
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
    }

    public void Resume()
    {
        _logger.Info("[Pause] Game Resumed");
        
        _isPaused = false;
        Time.timeScale = 1f;
        _pausePanel.SetActive(false);
    }

    private void QuitToTitle()
    {
        _logger.Info("[Pause] Quit to Title");
        
        DOTween.KillAll();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}