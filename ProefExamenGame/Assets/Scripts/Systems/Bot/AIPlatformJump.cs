using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI jumps to a platform (moving or rotating) when triggered.
/// Supports parenting for moving platforms and Rigidbody physics for rotating logs.
/// </summary>
public class AIPlatformJump : MonoBehaviour
{
    [Header("Target Platform")]
    public Transform _targetPlatform;

    [Header("Platform Type Settings")]
    [Tooltip("If true, AI will parent to platform (for moving cubes). If false, AI will use Rigidbody physics (for rotating logs).")]
    public bool _useParenting = true;

    [Header("Jump Settings")]
    public float _jumpDuration = 1.2f;
    public float _jumpHeight = 1.5f;
    public float _jumpDelay = 0.2f;

    private bool _isJumping;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bot")) return;
        if (_isJumping) return;
        if (_targetPlatform == null) return;

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Bot requires a Rigidbody.");
            return;
        }

        CapsuleCollider capsule = other.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            Debug.LogError("Bot requires a CapsuleCollider.");
            return;
        }

        StartCoroutine(JumpRoutine(other.transform, agent, rb, capsule));
    }

    private IEnumerator JumpRoutine(Transform ai, NavMeshAgent agent, Rigidbody rb, CapsuleCollider capsule)
    {
        _isJumping = true;

        yield return new WaitForSeconds(_jumpDelay);

        // Always detach from old platform
        ai.SetParent(null);

        // Disable NavMeshAgent during jump
        if (agent.enabled)
            agent.enabled = false;

        // Make Rigidbody kinematic during jump for smooth arc
        rb.isKinematic = true;

        Vector3 startPos = ai.position;
        float elapsed = 0f;

        // 🔹 Adjusted landing height (60% of half height instead of full half height)
        float botHalfHeight = capsule.bounds.extents.y * 0.05f;

        while (elapsed < _jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _jumpDuration;

            float platformTopY = GetPlatformTopY();
            Vector3 targetPos = _targetPlatform.position;
            targetPos.y = platformTopY + botHalfHeight;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += _jumpHeight * Mathf.Sin(Mathf.PI * t);

            ai.position = pos;

            yield return null;
        }

        // Final landing position
        float finalTopY = GetPlatformTopY();
        Vector3 finalPos = _targetPlatform.position;
        finalPos.y = finalTopY + botHalfHeight + 0.02f;
        ai.position = finalPos;

        // Apply platform behavior
        if (_useParenting)
        {
            // Moving platform → parent to platform
            ai.SetParent(_targetPlatform);
            rb.isKinematic = true;
        }
        else
        {
            // Rotating log → enable physics
            ai.SetParent(null);
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        _isJumping = false;
    }

    /// <summary>
    /// Returns the top Y world coordinate of the platform.
    /// </summary>
    private float GetPlatformTopY()
    {
        Renderer r = _targetPlatform.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.max.y;

        return _targetPlatform.position.y + (_targetPlatform.localScale.y * 0.5f);
    }
}