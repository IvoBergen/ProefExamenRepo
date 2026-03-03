using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moves the AI between multiple waypoint locations
/// </summary>
public class AIMoveNavMesh : MonoBehaviour
{
    public Transform[] Locations;

    private NavMeshAgent _agent;
    private int _currentIndex = 0;

    public bool IsJumping { get; set; } = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (Locations.Length > 0 && _agent.isOnNavMesh)
        {
            _agent.SetDestination(Locations[_currentIndex].position);
        }
    }

    void Update()
    {
        if (Locations.Length == 0) return;
        if (_agent == null) return;
        if (!_agent.enabled) return;
        if (!_agent.isOnNavMesh) return;

        // 🚫 Don't run patrol logic while jumping
        if (IsJumping) return;

        if (!_agent.pathPending &&
            _agent.remainingDistance <= _agent.stoppingDistance)
        {
            GoToNextLocation();
        }
    }

    void GoToNextLocation()
    {
        _currentIndex++;
        if (_currentIndex >= Locations.Length)
            _currentIndex = 0;

        _agent.SetDestination(Locations[_currentIndex].position);
    }
}