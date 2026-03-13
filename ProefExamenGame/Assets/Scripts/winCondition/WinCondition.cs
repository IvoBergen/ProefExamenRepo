using UnityEngine;
using UnityEngine.Events;

public class WinCondition : MonoBehaviour
{
    [SerializeField] UnityEvent Eventwin;
    [SerializeField] UnityEvent EventLose;

    [SerializeField] GameObject StatsUi;    // Win screen UI
    [SerializeField] GameObject UiLose;     // Lose screen UI

    [SerializeField] private PlayerStatsTracker playerStats; // Reference to player stats script

    private int _finishedAI;
    private int _totalAI;
    private int _loseThreshold;

    void Start()
    {
        // Count all AI in the scene
        AIMoveNavMesh[] allAI = FindObjectsOfType<AIMoveNavMesh>();
        _totalAI = allAI.Length;

        // 50% threshold (rounded up)
        _loseThreshold = Mathf.CeilToInt(_totalAI * 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bot"))
        {
            _finishedAI++;

            if (_finishedAI >= _loseThreshold)
            {
                TriggerLose();
            }
        }

        if (other.CompareTag("Player"))
        {
            TriggerWin();
        }
    }

    public void TriggerWin()
    {
        Eventwin.Invoke();

        if (playerStats != null)
        {
            playerStats.Win(); // Calls PlayerStatsTracker.Win() for win screen
        }

        if (StatsUi != null)
            StatsUi.SetActive(true);
    }

    public void TriggerLose()
    {
        EventLose.Invoke();

        if (playerStats != null)
        {
            playerStats.Lose(); // Calls PlayerStatsTracker.Lose() for lose screen
        }

        if (UiLose != null)
            UiLose.SetActive(true);
    }
}