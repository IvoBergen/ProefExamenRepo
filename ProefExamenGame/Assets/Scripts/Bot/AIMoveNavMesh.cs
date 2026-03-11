using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes the AI cycle through patrol points and handles jumping logic.
/// Includes waypoint offset to prevent AI stacking on the same location.
/// </summary>
public class AIMoveNavMesh : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] Locations;

    [Header("Idle Settings")]
    public float idleTime = 2f;

    [Header("Waypoint Settings")]
    [SerializeField] private float waypointOffsetRadius = 1.5f;

    private NavMeshAgent _agent;
    private int _currentIndex = 0;
    private bool _isWaiting = false;

    [SerializeField] private AIAnimator _animator;

    public bool IsJumping { get; private set; }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Randomize avoidance priority so AI don't deadlock
        _agent.avoidancePriority = Random.Range(20, 80);

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

        // Slightly larger arrival radius so AI don't fight for the same point
        if (!_agent.pathPending && _agent.remainingDistance <= 1.2f && !_isWaiting)
        {
            StartCoroutine(IdleRoutine());
        }
    }

    /// <summary>
    /// Moves to the next location with a small random offset.
    /// </summary>
    void MoveToNextLocation()
    {
        Vector3 offset = Random.insideUnitSphere * waypointOffsetRadius;
        offset.y = 0;

        Vector3 targetPosition = Locations[_currentIndex].position + offset;

        _agent.SetDestination(targetPosition);

        _animator?.SetBool("isWalking", true);
    }

    /// <summary>
    /// Idle before moving to next patrol point
    /// </summary>
    IEnumerator IdleRoutine()
    {
        _isWaiting = true;

        _agent.isStopped = true;
        _animator?.SetBool("isWalking", false);

        yield return new WaitForSeconds(idleTime);

        _currentIndex++;
        if (_currentIndex >= Locations.Length)
            _currentIndex = 0;

        _agent.isStopped = false;
        MoveToNextLocation();

        _isWaiting = false;
    }

    /// <summary>
    /// Handles walking animation
    /// </summary>
    void UpdateAnimation()
    {
        if (_agent.velocity.magnitude > 0.1f)
            _animator?.SetBool("isWalking", true);
        else if (!_isWaiting)
            _animator?.SetBool("isWalking", false);
    }

    // Jump Control

    /// <summary>
    /// Start jumping
    /// </summary>
    public void StartJump()
    {
        IsJumping = true;
        _agent.isStopped = true;

        _animator?.SetBool("isWalking", false);
        _animator?.SetBool("isJumping", true);
    }

    /// <summary>
    /// Stop jumping
    /// </summary>
    public void EndJump()
    {
        IsJumping = false;
        _agent.isStopped = false;

        if (Locations.Length > 0)
            MoveToNextLocation();
    }

    // Respawn Reset

    public void ResetPatrol()
    {
        StopAllCoroutines();

        _isWaiting = false;
        _currentIndex = 0;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (_agent.isOnNavMesh && Locations.Length > 0)
        {
            _agent.ResetPath();
            _agent.isStopped = false;
            MoveToNextLocation();
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}