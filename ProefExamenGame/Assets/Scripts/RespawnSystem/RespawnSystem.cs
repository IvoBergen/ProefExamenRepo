using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Handles killing volume logic and respawning players or bots.
/// Works with AIMoveNavMesh bots for patrol and jump reset.
/// </summary>
public class RespawnSystem : MonoBehaviour
{
    public UnityEvent died;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);

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

        // Disable NavMeshAgent BEFORE moving transform to prevent position conflict
        NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        // Reset and freeze rigidbody before teleporting
        Rigidbody rb = character.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Teleport character to checkpoint
        character.transform.SetPositionAndRotation(respawnPos, respawnRot);

        // Reset bot state if applicable
        AIMoveNavMesh bot = character.GetComponent<AIMoveNavMesh>();
        if (bot != null)
        {
            bot.ResetPatrol();
            bot.EndJump();

            if (agent != null)
            {
                // Re-enable agent then warp to sync NavMesh position
                agent.enabled = true;
                agent.Warp(respawnPos);
            }
        }

        // Re-enable physics after teleport
        if (rb != null)
            rb.isKinematic = false;
    }
}