using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AIMoveNavMesh))]
public class NavMeshJumpAgent : MonoBehaviour
{
    [SerializeField] private AIAnimator _animator;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private Collider _col;
    private NavMeshObstacle _obstacle;
    private AIMoveNavMesh _patrol;

    private bool _isJumping = false;
    private Coroutine _jumpCoroutine = null;

    [Header("Jump Settings")]
    public float jumpSpeed = 6f;
    public float minJumpDuration = 0.3f;
    public float maxJumpDuration = 2.0f;

    [Header("Fail System")]
    [Range(0f, 1f)]
    [SerializeField] private float _failChance = 0.1f;
    public float failLateralDrift = 1.2f;
    public float failDownwardNudge = 1.5f;
    public float failSettleTime = 1.5f;

    [Header("Debug")]
    public bool debugLogging = true;

    public delegate void JumpFailed();
    public event JumpFailed OnJumpFailed;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _obstacle = GetComponent<NavMeshObstacle>();
        _patrol = GetComponent<AIMoveNavMesh>();

        _agent.autoTraverseOffMeshLink = false;

        OnJumpFailed += RespawnMe;
    }

    private void Update()
    {
        if (_agent.isOnOffMeshLink && !_isJumping && _jumpCoroutine == null)
        {
            if (debugLogging)
                Debug.Log($"[NavMeshJumpAgent] {gameObject.name} triggering jump at {Time.time:F2}s | pos: {transform.position}");

            _jumpCoroutine = StartCoroutine(JumpAcross());
        }

        if (_isJumping && !_agent.updatePosition)
            _agent.nextPosition = transform.position;
    }

    private float CalcJumpDuration(Vector3 start, Vector3 end)
    {
        float dist = Vector3.Distance(start, end);
        return Mathf.Clamp(dist / jumpSpeed, minJumpDuration, maxJumpDuration);
    }

    IEnumerator JumpAcross()
    {
        _patrol?.StartJump();
        _animator.SetBool("isJumping", true);
        _animator.SetBool("isWalking", false);
        _isJumping = true;

        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 startPos = transform.position;
        float halfHeight = _agent.height * 0.5f;
        Vector3 endPos = data.endPos + Vector3.up * halfHeight;

        if (debugLogging)
            Debug.Log($"[NavMeshJumpAgent] {gameObject.name} jumping from {startPos} to {endPos}");

        _agent.isStopped = true;
        _agent.updatePosition = false;

        float jumpDuration = CalcJumpDuration(startPos, endPos);
        bool failed = Random.value < _failChance;

        Vector3 toEnd = endPos - startPos;
        Vector3 horizontalVelocity = new Vector3(toEnd.x, 0f, toEnd.z) / jumpDuration;
        float verticalVelocity = (toEnd.y / jumpDuration) - (0.5f * Physics.gravity.y * jumpDuration);
        Vector3 perfectVelocity = horizontalVelocity + Vector3.up * verticalVelocity;

        // ───────── FAILED JUMP ─────────
        if (failed)
        {
            if (debugLogging)
                Debug.Log($"[NavMeshJumpAgent] {gameObject.name} FAILED jump at {Time.time:F2}s");

            _failChance = 0.1f;
            _rb.isKinematic = false;
            _rb.velocity = perfectVelocity;

            float deflectTime = jumpDuration * 0.35f;
            yield return new WaitForSeconds(deflectTime);

            Vector3 currentVel = _rb.velocity;
            currentVel.x *= 0.4f;
            currentVel.z *= 0.4f;
            currentVel.y -= failDownwardNudge;

            Vector3 jumpDir = (endPos - startPos).normalized;
            Vector3 sideways = Vector3.Cross(jumpDir, Vector3.up).normalized;
            sideways *= (Random.value > 0.5f ? 1f : -1f);
            currentVel += sideways * failLateralDrift;

            _rb.velocity = currentVel;

            yield return new WaitForSeconds(failSettleTime);

            // ─── Reset after fail ───
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;

            // Move AI back to last patrol position
            transform.position = _agent.nextPosition; // ensures it doesn't get stuck in the air

            _agent.isStopped = false;               // allow movement again
            _agent.Warp(_agent.nextPosition);       // re-sync NavMeshAgent
            _patrol?.EndJump();                     // tells patrol we are done jumping
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isWalking", true);

            _isJumping = false;
            _jumpCoroutine = null;

            if (debugLogging)
                Debug.Log($"[NavMeshJumpAgent] {gameObject.name} jump FAILED complete at {Time.time:F2}s");

            yield break;
        }

        // ───────── SUCCESSFUL JUMP ─────────
        _failChance = Mathf.Min(_failChance * 2f, 1f);

        _rb.isKinematic = false;
        _rb.velocity = perfectVelocity;

        yield return new WaitForSeconds(jumpDuration);

        // LANDING
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.position = endPos;

        _agent.CompleteOffMeshLink();
        _agent.updatePosition = true;
        _agent.isStopped = false;

        _animator.SetBool("isJumping", false);
        _patrol?.EndJump();

        _isJumping = false;
        _jumpCoroutine = null;
    }

    private void RespawnMe()
    {
        if (CheckPointManager.Instance == null || CheckPointManager.Instance.CurrentCheckpoint == null)
            return;

        ResetAfterRespawn(CheckPointManager.Instance.CurrentCheckpoint.transform.position);
    }

    public void ResetAfterRespawn(Vector3 respawnPosition)
    {
        StopAllCoroutines();
        _isJumping = false;
        _jumpCoroutine = null;

        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;

        transform.position = respawnPosition;
        _agent.Warp(respawnPosition);
        _agent.updatePosition = true;
        _agent.isStopped = false;

        _patrol?.ResetPatrol();
        _animator.SetBool("isWalking", true);
    }
}