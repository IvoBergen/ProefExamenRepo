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
    [SerializeField] private float startTime = 120f;

    [Header("UI")]
    /// <summary>
    /// Legacy UI Text that displays the timer.
    /// </summary>
    [SerializeField] private Text timerText;

    [Header("Milestone Events")]
    /// <summary>Triggered when timer reaches 90 seconds remaining.</summary>
    public UnityEvent on90Seconds;
    /// <summary>Triggered when timer reaches 60 seconds remaining.</summary>
    public UnityEvent on60Seconds;
    /// <summary>Triggered when timer reaches 30 seconds remaining.</summary>
    public UnityEvent on30Seconds;
    /// <summary>Triggered when timer reaches 0.</summary>
    public UnityEvent onTimeUp;

    private float currentTime;
    private bool fired90, fired60, fired30, firedTimeUp;

    /// <summary>Initialize timer.</summary>
    private void Start()
    {
        currentTime = startTime;
        UpdateUI();
    }

    /// <summary>Countdown and milestone checking.</summary>
    private void Update()
    {
        if (firedTimeUp) return;

        currentTime -= Time.deltaTime;
        CheckMilestones();
        UpdateUI();

        if (currentTime <= 0)
        {
            currentTime = 0;
            firedTimeUp = true;
            onTimeUp.Invoke();
        }
    }

    /// <summary>Fire milestone events once.</summary>
    private void CheckMilestones()
    {
        if (!fired90 && currentTime <= 90)
        {
            fired90 = true;
            on90Seconds.Invoke();
        }
        if (!fired60 && currentTime <= 60)
        {
            fired60 = true;
            on60Seconds.Invoke();
        }
        if (!fired30 && currentTime <= 30)
        {
            fired30 = true;
            on30Seconds.Invoke();
        }
    }

    /// <summary>Convert seconds → MM:SS and update text.</summary>
    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}