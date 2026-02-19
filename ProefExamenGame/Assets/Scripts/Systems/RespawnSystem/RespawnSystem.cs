using UnityEngine;
using UnityEngine.Events;
public class RespawnSystem : MonoBehaviour
{
    public UnityEvent died;
    /// <summary>
    /// Handles respawning the player at the current checkpoint when they fall out of bounds or trigger a respawn area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Respawn(other.gameObject);
        }
    }

    /// <summary>
    /// Respawns the player at the current checkpoint, resetting their position, rotation, and physics state. and invokes the death event
    /// </summary>
    private void Respawn(GameObject player)
    {
        died.Invoke();
        if (CheckPointManager.Instance == null || CheckPointManager.Instance.CurrentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint set!");
            return;
        }

        Transform respawnLocation = CheckPointManager.Instance.CurrentCheckpoint.transform;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        player.transform.position = respawnLocation.position;
        player.transform.rotation = respawnLocation.rotation;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }
}
