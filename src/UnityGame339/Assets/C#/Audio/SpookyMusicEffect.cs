using Game.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class SpookyMusicEffect : MonoBehaviour
{
    [Header("Progression")]
    [Tooltip("Encounters completed before spookiness hits maximum")]
    [SerializeField] private int _encountersToMaxSpook = 10;
    [SerializeField] private AnimationCurve _spookinessCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Tooltip("How many seconds a spookiness transition takes")]
    [SerializeField] private float _transitionDuration = 4f;

    [Header("Low Pass Filter (Muffling)")]
    [SerializeField] private float _normalCutoff  = 22000f;
    [SerializeField] private float _spookyCutoff  = 1200f;

    [Header("Distortion (Fuzz)")]
    [SerializeField] private float _maxDistortion = 0.35f;

    [Header("Chorus (Warble)")]
    [SerializeField] private float _maxChorusDepth = 0.8f;
    [SerializeField] private float _maxChorusRate  = 1.2f;

    [Header("Pitch Wobble")]
    [SerializeField] private float _maxWobbleAmount = 0.04f;
    [SerializeField] private float _wobbleFrequency = 0.4f;

    // Audio filters
    private AudioSource           _audioSource;
    private AudioLowPassFilter    _lowPass;
    private AudioDistortionFilter _distortion;
    private AudioChorusFilter     _chorus;

    // State
    private TurnEngine _turnEngine;
    private float      _targetSpookiness;
    private float      _currentSpookiness;
    private float      _basePitch;
    private int        _totalEncounters = 0;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _basePitch   = _audioSource.pitch;

        _lowPass = gameObject.AddComponent<AudioLowPassFilter>();
        _lowPass.cutoffFrequency = _normalCutoff;

        _distortion = gameObject.AddComponent<AudioDistortionFilter>();
        _distortion.distortionLevel = 0f;
        _distortion.enabled = false;

        _chorus = gameObject.AddComponent<AudioChorusFilter>();
        _chorus.depth   = 0f;
        _chorus.rate    = 0f;
        _chorus.enabled = false;
    }

    private void Start()
    {
        _turnEngine = ServiceResolver.Resolve<TurnEngine>();
        _turnEngine.EncounterEnd += OnEncounterEnd;
    }

    private void OnDestroy()
    {
        if (_turnEngine != null)
            _turnEngine.EncounterEnd -= OnEncounterEnd;
    }

    private void OnEncounterEnd(bool isPlayerWin)
    {
        if (!isPlayerWin) return;
        _totalEncounters++;
        float t = Mathf.Clamp01((float)_totalEncounters / _encountersToMaxSpook);
        _targetSpookiness = _spookinessCurve.Evaluate(t);
    }

    private void Update()
    {
        _currentSpookiness = Mathf.MoveTowards(
            _currentSpookiness,
            _targetSpookiness,
            Time.deltaTime / _transitionDuration
        );

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            _targetSpookiness = 1f;
            _currentSpookiness = 1f;
        }

        bool isActive = _currentSpookiness > 0.01f;
        _distortion.enabled = isActive;
        _chorus.enabled     = isActive;

        _lowPass.cutoffFrequency    = Mathf.Lerp(_normalCutoff, _spookyCutoff, _currentSpookiness);
        _distortion.distortionLevel = Mathf.Lerp(0f, _maxDistortion, _currentSpookiness);
        _chorus.depth               = Mathf.Lerp(0f, _maxChorusDepth, _currentSpookiness);
        _chorus.rate                = Mathf.Lerp(0f, _maxChorusRate, _currentSpookiness);

        _audioSource.pitch = isActive
            ? _basePitch + Mathf.Sin(Time.time * _wobbleFrequency * (1f + _currentSpookiness * 2f))
                           * _maxWobbleAmount * _currentSpookiness
            : _basePitch;
    }
}