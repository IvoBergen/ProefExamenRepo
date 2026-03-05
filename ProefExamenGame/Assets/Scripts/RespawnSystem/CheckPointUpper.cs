using UnityEngine;

public class CheckPointUpper : MonoBehaviour
{
    /// <summary>
    /// Detects when a player reaches a checkpoint and notifies the CheckPointManager to activate the next checkpoint.
    /// </summary>
    [SerializeField] CheckPointManager _manager;
    private void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("NextCheckPoint");
            _manager.ActivateNextCheckpoint();
            Destroy(gameObject);
        }
    }
}
