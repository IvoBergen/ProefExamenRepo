using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes the navmesh agent jump across off-mesh links instantly on contact,
/// with distance-based duration, occupied link avoidance, and respawn on failure.
/// </summary>
public class NavMeshJumpAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private Collider _col;
    private NavMeshObstacle _obstacle;
    private bool _isJumping = false;
    private ObstacleAvoidanceType _originalAvoidance;
    private int _originalPriority;

    [Header("Jump Settings")]
    [Tooltip("How fast the agent travels horizontally across a jump (units/sec). Duration = distance / speed.")]
    public float jumpSpeed = 6f;
    [Tooltip("Minimum allowed jump duration, regardless of distance.")]
    public float minJumpDuration = 0.3f;
    [Tooltip("Maximum allowed jump duration, regardless of distance.")]
    public float maxJumpDuration = 2.0f;

    [Header("Link Avoidance")]
    [Tooltip("Radius to scan for off-mesh links when picking the best free one.")]
    public float linkScanRadius = 20f;
    [Tooltip("How long to wait before retrying if all nearby links are occupied.")]
    public float retryDelay = 0.2f;

    [Header("Fail System")]
    [Range(0f, 1f)]
    [SerializeField] private float _failChance = 0.1f;

    public delegate void JumpFailed();
    public event JumpFailed OnJumpFailed;

    // shared across all agents — tracks which link endpoints are currently in use
    private static readonly HashSet<Vector3> _occupiedLinks = new HashSet<Vector3>();

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _obstacle = GetComponent<NavMeshObstacle>();

        _originalAvoidance = _agent.obstacleAvoidanceType;
        _originalPriority = _agent.avoidancePriority;

        _agent.autoTraverseOffMeshLink = false;

        OnJumpFailed += RespawnMe;
    }

    private void Update()
    {
        if (_agent.isOnOffMeshLink && !_isJumping)
        {
            StartCoroutine(JumpAcross());
        }
    }

    // ─────────────────────────────────────────────
    //  Occupied link registry
    // ─────────────────────────────────────────────

    private static Vector3 RoundVec(Vector3 v) =>
        new Vector3(Mathf.Round(v.x * 10) / 10f,
                    Mathf.Round(v.y * 10) / 10f,
                    Mathf.Round(v.z * 10) / 10f);

    private static bool IsLinkOccupied(Vector3 endPos) =>
        _occupiedLinks.Contains(RoundVec(endPos));

    private static void ClaimLink(Vector3 endPos) =>
        _occupiedLinks.Add(RoundVec(endPos));

    private static void ReleaseLink(Vector3 endPos) =>
        _occupiedLinks.Remove(RoundVec(endPos));

    // ─────────────────────────────────────────────
    //  State helpers
    // ─────────────────────────────────────────────

    private void SetJumpingState(bool jumping)
    {
        if (_col != null) _col.enabled = !jumping;
        if (_obstacle != null) _obstacle.enabled = !jumping;

        if (jumping)
        {
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _agent.avoidancePriority = 0;
        }
        else
        {
            _agent.obstacleAvoidanceType = _originalAvoidance;
            _agent.avoidancePriority = _originalPriority;
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
        Vector3 endPos = data.endPos + Vector3.up * 0.1f;

        // ── wait until this specific link is free ──
        while (IsLinkOccupied(endPos))
        {
            // step aside so others don't stack on the same spot
            _agent.isStopped = true;
            yield return new WaitForSeconds(retryDelay);

            // if we got bumped off the link somehow, abort
            if (!_agent.isOnOffMeshLink)
            {
                _agent.isStopped = false;
                _isJumping = false;
                yield break;
            }
        }

        ClaimLink(endPos);
        SetJumpingState(true);

        _agent.isStopped = true;
        _agent.updatePosition = false;

        float jumpDuration = CalcJumpDuration(startPos, endPos);
        bool failed = Random.value < _failChance;

        Vector3 perfectVelocity = (endPos - startPos) / jumpDuration;
        perfectVelocity.y = jumpDuration * 0.5f * -Physics.gravity.y;

        // ───────── FAILED JUMP ─────────
        if (failed)
        {
            _failChance = 0.1f;
            _rb.isKinematic = false;
            _rb.velocity = perfectVelocity * 0.95f;

            yield return new WaitForSeconds(jumpDuration * 0.45f);

            Vector3 v = _rb.velocity;
            v.x *= 0.6f;
            v.z *= 0.6f;
            v.y *= 0.5f;
            _rb.velocity = v;

            yield return new WaitForSeconds(1.3f);

            ReleaseLink(endPos);
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

            ReleaseLink(endPos);
        }

        SetJumpingState(false);
        _isJumping = false;
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

        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;

        SetJumpingState(false);

        transform.position = respawnPosition;
        _agent.Warp(respawnPosition);
        _agent.updatePosition = true;
        _agent.isStopped = false;
    }
}