using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class RespawnSystem : MonoBehaviour
{
    /// <summary>
    /// Handles killing volume logic and respawning players or bots.
    /// Works with AIMoveNavMesh bots for patrol and jump reset.
    /// </summary>
    public UnityEvent died;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Bot"))
            return;

        RespawnCharacter(other.gameObject);

        if (other.CompareTag("Player"))
            died?.Invoke();
    }

    /// <summary>
    /// Respawns any character (player or bot) and resets physics + navmesh state.
    /// Works with AIMoveNavMesh bots.
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
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        character.transform.SetPositionAndRotation(respawnPos, respawnRot);
        AIMoveNavMesh bot = character.GetComponent<AIMoveNavMesh>();
        if (bot != null)
        {
            bot.ResetPatrol();
            bot.EndJump();
            NavMeshAgent agent = bot.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(respawnPos);
            }
        }
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}