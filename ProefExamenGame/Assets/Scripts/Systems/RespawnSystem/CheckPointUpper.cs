using UnityEngine;

public class CheckPointUpper : MonoBehaviour
{
    [SerializeField] CheckPointManager manager;
    private void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("NextCheckPoint");
            manager.ActivateNextCheckpoint();
        }
    }
}
