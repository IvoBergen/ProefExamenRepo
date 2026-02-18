using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshJumpAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    public float jumpForce = 8f;
    public float jumpDuration = 0.5f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_agent.isOnOffMeshLink)
        {
            StartCoroutine(JumpAcross());
        }
    }

    IEnumerator JumpAcross()
    {
        _agent.isStopped = true;
        _agent.updatePosition = false;

        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * 0.1f;

        // jump physics
        _rb.isKinematic = false;
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpDuration);

        // teleport to end of link
        transform.position = endPos;

        _rb.isKinematic = true;

        _agent.CompleteOffMeshLink();
        _agent.updatePosition = true;
        _agent.isStopped = false;
    }
}
