using UnityEngine;
using UnityEngine.Events;
public class RespawnSystem : MonoBehaviour
{
    public UnityEvent died;
    /// <summary>
    /// Triggered when player or bots enter the kill volume.
    /// Respawns them at the current checkpoint.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            died.Invoke();
        }
        if (other.CompareTag("Player") || other.CompareTag("Bot"))
        {
            RespawnCharacter(other.gameObject);
        }
    }

    /// <summary>
    /// Respawns any character (player or bot) and resets physics + navmesh state.
    /// </summary>
    private void RespawnCharacter(GameObject character)
    {
        if (CheckPointManager.Instance == null || CheckPointManager.Instance.CurrentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint set!");
            return;
        }

        Transform checkpoint = CheckPointManager.Instance.CurrentCheckpoint.transform;
        Vector3 respawnPos = checkpoint.position;
        Quaternion respawnRot = checkpoint.rotation;

        Rigidbody rb = character.GetComponent<Rigidbody>();

        // Disable physics while teleporting
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teleport
        character.transform.SetPositionAndRotation(respawnPos, respawnRot);

        // If this character is a bot, reset its NavMeshAgent correctly
        NavMeshJumpAgent bot = character.GetComponent<NavMeshJumpAgent>();
        if (bot != null)
        {
            bot.ResetAfterRespawn(respawnPos);
        }

        // Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
