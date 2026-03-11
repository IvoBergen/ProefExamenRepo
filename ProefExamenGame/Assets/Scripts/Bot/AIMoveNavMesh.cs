using System.Collections;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Makes the ai cycle trough patrol points also handels jumping logic to turn off the agent
/// </summary>
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
    /// <summary>
    /// Moves to the next location and checks velocity of the agent and sets it to zero
    /// </summary>
    void MoveToNextLocation()
    {
        _agent.SetDestination(Locations[_currentIndex].position);
        _animator?.SetBool("isWalking", true);


        if (_agent.velocity.sqrMagnitude >= 20f * 20f)
        {
            _agent.velocity = Vector3.zero;
        }
    }
    /// <summary>
    /// set idle
    /// </summary>
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
    /// <summary>
    /// handels animation
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
    /// stops jumping
    /// </summary>
    public void EndJump()
    {
        IsJumping = false;
        _agent.isStopped = false;

        if (Locations.Length > 0)
            _agent.SetDestination(Locations[_currentIndex].position);
    }

    // Respawn Reset


    public void ResetPatrol()
    {
        StopAllCoroutines();

        _isWaiting = false;
        _currentIndex = 0;

        // Stop Rigidbody motion
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;  // temporarily kinematic to prevent physics interference
        }

        if (_agent.isOnNavMesh && Locations.Length > 0)
        {
            _agent.isStopped = false;
            MoveToNextLocation();
        }

        // Re-enable physics after a short frame to avoid glitches
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}