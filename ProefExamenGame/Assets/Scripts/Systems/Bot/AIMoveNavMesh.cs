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

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (Locations.Length > 0)
        {
            _agent.SetDestination(Locations[_currentIndex].position);
        }
    }

    void Update()
    {
        if (Locations.Length == 0) return;
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            GoToNextLocation();
        }
    }
    // set de volgende locatie van de AI
    void GoToNextLocation()
    {
        _currentIndex++;
        if (_currentIndex >= Locations.Length)
            _currentIndex = 0;

        _agent.SetDestination(Locations[_currentIndex].position);
    }
}
