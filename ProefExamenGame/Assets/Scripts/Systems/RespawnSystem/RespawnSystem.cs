using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Handles killing volume logic and respawning players or bots.
/// Ensures bot NavMeshAgents are re-enabled after respawn.
/// </summary>
public class RespawnSystem : MonoBehaviour
{
    public UnityEvent died;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Bot"))
            return;

        RespawnCharacter(other.gameObject);

        if (!other.CompareTag("Player")) return;
        died?.Invoke();
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
            NavMeshAgent agent = bot.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;          // Re-enable agent
                agent.Warp(respawnPos);        // Correct internal NavMesh position
            }
        }

        // Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}