using UnityEngine;
using UnityEngine.AI;

public class AIMoveNavMesh : MonoBehaviour
{
    public Transform[] Locations;
    private NavMeshAgent _agent;
    private int _currentIndex = 0;
    [SerializeField] private AIAnimator _animator;
    public bool IsJumping { get; set; } = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (Locations.Length > 0 && _agent.isOnNavMesh)
        {
            _agent.SetDestination(Locations[_currentIndex].position);
            _animator?.SetBool("isWalking", true);
        }
    }

    void Update()
    {
        if (Locations.Length == 0 || _agent == null || !_agent.enabled || !_agent.isOnNavMesh || IsJumping)
            return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
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
        _animator?.SetBool("isWalking", true);
    }
}