using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMoveNavMesh : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] Locations;

    [Header("Idle Settings")]
    public float idleTime = 2f;

    private NavMeshAgent _agent;
    private int _currentIndex = 0;
    private bool _isWaiting = false;

    [SerializeField] private AIAnimator _animator;

    public bool IsJumping { get; private set; }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (Locations.Length > 0 && _agent.isOnNavMesh)
        {
            MoveToNextLocation();
        }
    }

    void Update()
    {
        if (Locations.Length == 0 || !_agent.enabled || !_agent.isOnNavMesh || IsJumping)
            return;

        UpdateAnimation();

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance && !_isWaiting)
        {
            StartCoroutine(IdleRoutine());
        }
    }

    void MoveToNextLocation()
    {
        _agent.SetDestination(Locations[_currentIndex].position);
        _animator?.SetBool("isWalking", true);
    }

    IEnumerator IdleRoutine()
    {
        _isWaiting = true;
        _agent.isStopped = true;
        _animator?.SetBool("isWalking", false);

        yield return new WaitForSeconds(idleTime);

        _currentIndex++;
        if (_currentIndex >= Locations.Length) _currentIndex = 0;

        _agent.isStopped = false;
        MoveToNextLocation();

        _isWaiting = false;
    }

    void UpdateAnimation()
    {
        if (_agent.velocity.magnitude > 0.1f)
            _animator?.SetBool("isWalking", true);
        else if (!_isWaiting)
            _animator?.SetBool("isWalking", false);
    }

    // -------------------------
    // Jump Control
    // -------------------------

    public void StartJump()
    {
        IsJumping = true;
        _agent.isStopped = true;
        _animator?.SetBool("isWalking", false);
    }

    public void EndJump()
    {
        IsJumping = false;
        _agent.isStopped = false;

        if (Locations.Length > 0)
            _agent.SetDestination(Locations[_currentIndex].position);
    }

    // -------------------------
    // Respawn Reset
    // -------------------------

    public void ResetPatrol()
    {
        StopAllCoroutines();

        _isWaiting = false;
        _currentIndex = 0;

        if (_agent.isOnNavMesh && Locations.Length > 0)
        {
            _agent.isStopped = false;
            MoveToNextLocation();
        }
    }
}