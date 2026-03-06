using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlatformJump : MonoBehaviour
{
    /// <summary>
    /// makes the ai bot jump has a chance to fail the jump also
    /// </summary>
    [Header("Target Platform")]
    public Transform _targetPlatform;

    [Header("Platform Type Settings")]
    public bool _useParenting = true;

    [Header("Jump Settings")]
    public float _jumpDuration = 1.2f;
    public float _jumpHeight = 1.5f;
    public float _jumpDelay = 0.5f;

    [Header("Fail Settings")]
    [Range(0f, 1f)]
    public float _failChance = 0.15f;
    [Range(0f, 1f)]
    public float _failAtPercent = 0.6f;
    public float _failRespawnDelay = 5f;
    public float _failForwardLoss = 0.4f;

    [Header("Stuck Check")]
    public float _stuckTime = 10f; // seconds to check if bot didn't trigger another jump

    private HashSet<Transform> _jumpingBots = new HashSet<Transform>();
    private Dictionary<Transform, float> _lastJumpTime = new Dictionary<Transform, float>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bot") || _targetPlatform == null) return;
        if (_jumpingBots.Contains(other.transform)) return;

        Transform bot = other.transform;

        NavMeshAgent agent = bot.GetComponent<NavMeshAgent>();
        Rigidbody rb = bot.GetComponent<Rigidbody>();
        CapsuleCollider cap = bot.GetComponent<CapsuleCollider>();
        AIMoveNavMesh patrol = bot.GetComponent<AIMoveNavMesh>();
        AIAnimator animator = bot.GetComponent<AIAnimator>();

        if (agent == null || rb == null || cap == null) return;

        _jumpingBots.Add(bot);
        _lastJumpTime[bot] = Time.time;

        StartCoroutine(JumpRoutine(bot, agent, rb, cap, patrol, animator));
        StartCoroutine(StuckRespawnRoutine(bot, agent, rb, patrol, animator));
    }

    private IEnumerator JumpRoutine(
        Transform ai,
        NavMeshAgent agent,
        Rigidbody rb,
        CapsuleCollider cap,
        AIMoveNavMesh patrol,
        AIAnimator animator)
    {
        yield return new WaitForSeconds(_jumpDelay);

        animator?.SetBool("isWalking", false);
        animator?.SetBool("isJumping", true);

        patrol?.StartJump();

        yield return new WaitForSeconds(0.5f); // animation buffer

        Vector3 lookDir = _targetPlatform.position - ai.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            ai.rotation = Quaternion.LookRotation(lookDir);

        ai.SetParent(null);
        agent.enabled = false;
        rb.isKinematic = true;

        Vector3 startPos = ai.position;
        Vector3 targetPos = _targetPlatform.position;
        float botHalfHeight = cap.bounds.extents.y * 0.05f;
        targetPos.y = GetPlatformTopY() + botHalfHeight;

        float elapsed = 0f;
        bool failed = Random.value < _failChance;
        Vector3 lastPos = startPos;
        Vector3 velocity = Vector3.zero;

        while (elapsed < _jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _jumpDuration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += Mathf.Sin(Mathf.PI * t) * _jumpHeight * 1.2f;

            velocity = (pos - lastPos) / Time.deltaTime;
            lastPos = pos;
            ai.position = pos;

            if (failed && t >= _failAtPercent)
                break;

            yield return null;
        }

        if (failed)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            velocity *= _failForwardLoss;
            rb.velocity = velocity;

            ai.Rotate(Vector3.forward * Random.Range(-15f, 15f));
            yield return new WaitForSeconds(_failRespawnDelay);

            RespawnBot(ai, agent, rb, patrol, animator);
        }
        else
        {
            ai.position = targetPos;
            if (_useParenting)
            {
                ai.SetParent(_targetPlatform);
                rb.isKinematic = true;
            }
            else
            {
                ai.SetParent(null);
                rb.isKinematic = false;
            }

            patrol?.EndJump();
            animator?.SetBool("isJumping", false);
            animator?.SetBool("isWalking", true);
        }

        _jumpingBots.Remove(ai);
        _lastJumpTime.Remove(ai);
    }

    private IEnumerator StuckRespawnRoutine(
        Transform ai,
        NavMeshAgent agent,
        Rigidbody rb,
        AIMoveNavMesh patrol,
        AIAnimator animator)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (!_jumpingBots.Contains(ai)) yield break;

            float lastJump = _lastJumpTime.ContainsKey(ai) ? _lastJumpTime[ai] : 0f;

            if (!agent.isActiveAndEnabled && Time.time - lastJump >= _stuckTime)
            {
                RespawnBot(ai, agent, rb, patrol, animator);
                _jumpingBots.Remove(ai);
                _lastJumpTime.Remove(ai);
                yield break;
            }
        }
    }

    private void RespawnBot(Transform ai, NavMeshAgent agent, Rigidbody rb, AIMoveNavMesh patrol, AIAnimator animator)
    {
        if (CheckPointManager.Instance != null && CheckPointManager.Instance.CurrentCheckpoint != null)
        {
            Transform respawn = CheckPointManager.Instance.CurrentCheckpoint.transform;

            ai.position = respawn.position;
            agent.Warp(respawn.position);
            agent.enabled = true;

            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;

            patrol?.ResetPatrol();
            animator?.SetBool("isJumping", false);
            animator?.SetBool("isWalking", true);
        }
    }

    private float GetPlatformTopY()
    {
        Renderer r = _targetPlatform.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.max.y;

        return _targetPlatform.position.y + (_targetPlatform.localScale.y * 0.5f);
    }
}