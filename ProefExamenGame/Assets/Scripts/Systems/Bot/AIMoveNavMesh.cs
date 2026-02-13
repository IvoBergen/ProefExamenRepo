using UnityEngine;
using UnityEngine.AI;

public class AIMoveNavMesh : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            _agent.SetDestination(player.position);
        }
    }
}
