using UnityEngine;

public class CheckPointUpper : MonoBehaviour
{
    /// <summary>
    /// Detects when a player reaches a checkpoint and notifies the CheckPointManager to activate the next checkpoint.
    /// </summary>
    [SerializeField] CheckPointManager manager;
    private void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("NextCheckPoint");
            manager.ActivateNextCheckpoint();
            Destroy(gameObject);
        }
    }
}
