using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// is used at the finale jump to reactivate the agent
/// </summary>
public class AIEnableNavMeshOnGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bot")) return;

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        if (!agent.enabled)
        {
            agent.enabled = true;
        }
    }
}