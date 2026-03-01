using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes the navmesh agent jump across off-mesh links instantly on contact,
/// with distance-based duration and respawn on failure.
/// </summary>
public class NavMeshJumpAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private Collider _col;
    private NavMeshObstacle _obstacle;
    private bool _isJumping = false;
    private Coroutine _jumpCoroutine = null;
    private ObstacleAvoidanceType _originalAvoidance;
    private int _originalPriority;
    private int _originalLayer;

    [Header("Jump Settings")]
    [Tooltip("How fast the agent travels horizontally across a jump (units/sec). Duration = distance / speed.")]
    public float jumpSpeed = 6f;
    [Tooltip("Minimum allowed jump duration, regardless of distance.")]
    public float minJumpDuration = 0.3f;
    [Tooltip("Maximum allowed jump duration, regardless of distance.")]
    public float maxJumpDuration = 2.0f;

    [Header("Fail System")]
    [Range(0f, 1f)]
    [SerializeField] private float _failChance = 0.1f;
    [Tooltip("How far sideways the agent drifts on a subtle failed jump.")]
    public float failLateralDrift = 1.2f;
    [Tooltip("Extra downward nudge applied on fail to pull the agent off the platform naturally.")]
    public float failDownwardNudge = 1.5f;
    [Tooltip("How long after the stumble before we give up and respawn.")]
    public float failSettleTime = 1.5f;

    [Header("Collision")]
    [Tooltip("Create a layer called 'JumpingAgent' in Unity and disable its collision with your Agent layer in Physics settings.")]
    public bool useJumpingLayer = true;

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

        _originalAvoidance = _agent.obstacleAvoidanceType;
        _originalPriority = _agent.avoidancePriority;
        _originalLayer = gameObject.layer;

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
        {
            _agent.nextPosition = transform.position;
        }
    }

    // ─────────────────────────────────────────────
    //  State helpers
    // ─────────────────────────────────────────────

    private void SetJumpingState(bool jumping)
    {
        if (useJumpingLayer)
        {
            if (jumping)
            {
                int jumpLayer = LayerMask.NameToLayer("JumpingAgent");
                if (jumpLayer >= 0) gameObject.layer = jumpLayer;
            }
            else
            {
                gameObject.layer = _originalLayer;
            }
        }

        if (jumping)
        {
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _agent.avoidancePriority = 0;
            if (_obstacle != null) _obstacle.enabled = false;
        }
        else
        {
            _agent.obstacleAvoidanceType = _originalAvoidance;
            _agent.avoidancePriority = _originalPriority;
            if (_obstacle != null) _obstacle.enabled = true;
        }
    }

    private float CalcJumpDuration(Vector3 start, Vector3 end)
    {
        float dist = Vector3.Distance(start, end);
        return Mathf.Clamp(dist / jumpSpeed, minJumpDuration, maxJumpDuration);
    }

    // ─────────────────────────────────────────────
    //  Jump coroutine
    // ─────────────────────────────────────────────

    IEnumerator JumpAcross()
    {
        _isJumping = true;

        OffMeshLinkData data = _agent.currentOffMeshLinkData;
        Vector3 startPos = transform.position;

        float halfHeight = _agent.height * 0.5f;
        Vector3 endPos = data.endPos + Vector3.up * halfHeight;

        if (debugLogging)
            Debug.Log($"[NavMeshJumpAgent] {gameObject.name} jumping from {startPos} to {endPos}");

        SetJumpingState(true);

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

            SetJumpingState(false);
            _isJumping = false;
            _jumpCoroutine = null;
            OnJumpFailed?.Invoke();
            yield break;
        }

        // ───────── SUCCESSFUL JUMP ─────────
        if (debugLogging)
            Debug.Log($"[NavMeshJumpAgent] {gameObject.name} SUCCESS jump, duration: {jumpDuration:F2}s");

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

        SetJumpingState(false);
        _isJumping = false;
        _jumpCoroutine = null;

        if (debugLogging)
            Debug.Log($"[NavMeshJumpAgent] {gameObject.name} completed jump at {Time.time:F2}s");
    }

    // ─────────────────────────────────────────────
    //  Respawn
    // ─────────────────────────────────────────────

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

        SetJumpingState(false);

        transform.position = respawnPosition;
        _agent.Warp(respawnPosition);
        _agent.updatePosition = true;
        _agent.isStopped = false;
    }
}