using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshJumpAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private bool _isJumping = false;

    [Header("Jump Settings")]
    public float jumpDuration = 0.6f;

    [Header("Fail System")]
    [Range(0f, 1f)]
    [SerializeField] private float _failChance = 0.1f;

    public delegate void JumpFailed();
    public event JumpFailed OnJumpFailed;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();

        OnJumpFailed += RespawnMe;
    }

    private void Update()
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

        bool failed = Random.value < _failChance;

        // Calculate the PERFECT jump velocity first
        Vector3 perfectVelocity = (endPos - startPos) / jumpDuration;
        perfectVelocity.y = jumpDuration * 0.5f * -Physics.gravity.y;

        // ───────── FAILED JUMP (WEAK JUMP) ─────────
        if (failed)
        {
            _failChance = 0.1f;

            _rb.isKinematic = false;

            // Start with almost correct jump (95% power)
            Vector3 weakVelocity = perfectVelocity * 0.95f;
            _rb.velocity = weakVelocity;

            // After part of the arc → lose a bit more momentum
            yield return new WaitForSeconds(jumpDuration * 0.45f);

            Vector3 v = _rb.velocity;

            // Reduce forward speed a bit more
            v.x *= 0.6f;
            v.z *= 0.6f;

            // Reduce upward force so gravity wins earlier
            v.y *= 0.5f;

            _rb.velocity = v;

            // Let physics finish the fall naturally
            yield return new WaitForSeconds(1.3f);

            OnJumpFailed?.Invoke();
        }

        // ───────── SUCCESSFUL JUMP ─────────
        else
        {
            _failChance = Mathf.Min(_failChance * 2f, 1f);

            _rb.isKinematic = false;
            _rb.velocity = perfectVelocity;

            yield return new WaitForSeconds(jumpDuration);

            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;

            transform.position = endPos;

            _agent.CompleteOffMeshLink();
            _agent.updatePosition = true;
            _agent.isStopped = false;
        }

        _isJumping = false;
    }

    private void RespawnMe()
    {
        if (CheckPointManager.Instance == null || CheckPointManager.Instance.CurrentCheckpoint == null)
            return;

        Vector3 respawnPos = CheckPointManager.Instance.CurrentCheckpoint.transform.position;
        ResetAfterRespawn(respawnPos);
    }

    public void ResetAfterRespawn(Vector3 respawnPosition)
    {
        StopAllCoroutines();
        _isJumping = false;

        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;

        transform.position = respawnPosition;

        _agent.Warp(respawnPosition);
        _agent.updatePosition = true;
        _agent.isStopped = false;
    }
}
