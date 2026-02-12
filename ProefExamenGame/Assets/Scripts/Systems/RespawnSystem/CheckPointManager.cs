using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    /// <summary>
    /// Manages all checkpoints in the scene.
    /// Handles activating the next checkpoint and tracking the current checkpoint.
    /// Uses a singleton pattern for global access.
    /// </summary>
    public static CheckPointManager Instance { get; private set; }

    public CheckPoint[] allCheckPoints; // drag checkpoints in Inspector in order
    private int _currentIndex = 0;

    public CheckPoint CurrentCheckpoint => allCheckPoints.Length > 0 ? allCheckPoints[_currentIndex] : null;

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
    /// <summary>
    /// Sets the index to the next checkpoint
    /// </summary>
    public void ActivateNextCheckpoint()
    {
        if (_currentIndex + 1 < allCheckPoints.Length)
        {
            _currentIndex++;
            Debug.Log("Checkpoint activated: " + _currentIndex);
        }
    }
    /// <summary>
    /// makes sure the index stays inside of the bounds 
    /// </summary>
    public void SetCheckpoint(int index)
    {
        if (index >= 0 && index < allCheckPoints.Length)
        {
            _currentIndex = index;
        }
    }
}
