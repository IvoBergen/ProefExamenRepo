using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used to trigger the win condition
/// </summary>
public class WinCondition : MonoBehaviour
{
    [SerializeField] UnityEvent Eventwin;
    [SerializeField] UnityEvent EventLose;

    [SerializeField] GameObject StatsUi;
    [SerializeField] GameObject UiLose;

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
                EventLose.Invoke();
                UiLose.SetActive(true);
            }
        }

        if (other.CompareTag("Player"))
        {
            Win();
        }
    }

    /// <summary>
    /// Activates the win event.
    /// </summary>
    void Win()
    {
        Eventwin.Invoke();
        StatsUi.SetActive(true);
    }
}