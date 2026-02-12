using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Mario-style countdown timer using Legacy UI Text.
/// Counts down from a start time, updates UI, and fires milestone events.
/// </summary>
public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    /// <summary>
    /// Total time the level starts with (in seconds).
    /// Default = 120 seconds (2 minutes).
    /// </summary>
    [SerializeField] private float _startTime = 120f;

    [Header("UI")]
    /// <summary>
    /// Legacy UI Text that displays the timer.
    /// </summary>
    [SerializeField] private Text _timerText;

    [Header("Milestone Events")]
    /// <summary>Triggered when timer reaches 90 seconds remaining.</summary>
    public UnityEvent on90Seconds;
    /// <summary>Triggered when timer reaches 60 seconds remaining.</summary>
    public UnityEvent on60Seconds;
    /// <summary>Triggered when timer reaches 30 seconds remaining.</summary>
    public UnityEvent on30Seconds;
    /// <summary>Triggered when timer reaches 0.</summary>
    public UnityEvent onTimeUp;

    private float _currentTime;
    private bool _fired90, _fired60, _fired30, _firedTimeUp;

    /// <summary>Initialize timer.</summary>
    private void Start()
    {
        _currentTime = _startTime;
        UpdateUI();
    }

    /// <summary>Countdown and milestone checking.</summary>
    private void Update()
    {
        if (_firedTimeUp) return;

        _currentTime -= Time.deltaTime;
        CheckMilestones();
        UpdateUI();

        if (_currentTime <= 0)
        {
            _currentTime = 0;
            _firedTimeUp = true;
            onTimeUp.Invoke();
        }
    }

    /// <summary>Fire milestone events once.</summary>
    private void CheckMilestones()
    {
        if (!_fired90 && _currentTime <= 90)
        {
            _fired90 = true;
            on90Seconds.Invoke();
        }
        if (!_fired60 && _currentTime <= 60)
        {
            _fired60 = true;
            on60Seconds.Invoke();
        }
        if (!_fired30 && _currentTime <= 30)
        {
            _fired30 = true;
            on30Seconds.Invoke();
        }
    }

    /// <summary>Convert seconds → MM:SS and update text.</summary>
    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(_currentTime / 60);
        int seconds = Mathf.FloorToInt(_currentTime % 60);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }
}