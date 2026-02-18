using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshJumpAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private bool _isJumping = false;

    public float jumpForce = 8f;
    public float jumpDuration = 0.5f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_agent.isOnOffMeshLink && !_isJumping)
        {
            StartCoroutine(JumpAcross());
        }
    }

    IEnumerator JumpAcross()
    {
        _isJumping = true;
        _agent.isStopped = true;
        _agent.updatePosition = false;

        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos + Vector3.up * 0.1f;

        // Calculate velocity needed to reach endPos in jumpDuration
        Vector3 velocity = (endPos - startPos) / jumpDuration;
        velocity.y = jumpDuration * 0.5f * -Physics.gravity.y; // arc upward

        _rb.isKinematic = false;
        _rb.velocity = velocity;

        yield return new WaitForSeconds(jumpDuration);

        // Snap to end of link
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.position = endPos;

        _agent.CompleteOffMeshLink();
        _agent.updatePosition = true;
        _agent.isStopped = false;

        _isJumping = false;
    }
}