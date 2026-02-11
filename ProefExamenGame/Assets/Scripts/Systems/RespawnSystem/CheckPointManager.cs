using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; }

    public CheckPoint[] allCheckPoints; // drag checkpoints in Inspector in order
    private int currentIndex = 0;

    public CheckPoint CurrentCheckpoint => allCheckPoints.Length > 0 ? allCheckPoints[currentIndex] : null;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ActivateNextCheckpoint()
    {
        if (currentIndex + 1 < allCheckPoints.Length)
        {
            currentIndex++;
            Debug.Log("Checkpoint activated: " + currentIndex);
        }
    }

    public void SetCheckpoint(int index)
    {
        if (index >= 0 && index < allCheckPoints.Length)
        {
            currentIndex = index;
        }
    }
}
