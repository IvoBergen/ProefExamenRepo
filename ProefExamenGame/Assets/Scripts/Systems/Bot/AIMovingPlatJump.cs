using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMovingPlatJump : MonoBehaviour
{
    [Header("Platform settings")]
    public MovingCubes _movingPlatform;

    [Header("Jump settings")]
    public float _jumpDuration = 1.2f;
    public float _archHeight = 1.5f;
    public float _jumpDelay = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (_movingPlatform == null) return;
        if (!other.CompareTag("Bot")) return;

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        AIMoveNavMesh patrol = other.GetComponent<AIMoveNavMesh>();
        if (patrol == null) return;

        StartCoroutine(JumpRoutine(other.transform, agent, patrol));
    }

    private IEnumerator JumpRoutine(Transform aiTransform, NavMeshAgent agent, AIMoveNavMesh patrol)
    {
        // korte delay voor sprong
        yield return new WaitForSeconds(_jumpDelay);

        patrol.IsJumping = true;

        // 🔥 agent volledig uitzetten
        agent.enabled = false;

        Vector3 startPos = aiTransform.position;
        float elapsed = 0f;

        Renderer rend = _movingPlatform.GetComponent<Renderer>();
        float platformHeight = rend != null ? rend.bounds.size.y : _movingPlatform.transform.localScale.y;

        Quaternion uprightRotation = Quaternion.Euler(0f, aiTransform.eulerAngles.y, 0f);

        // Sprong
        while (elapsed < _jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _jumpDuration;

            Vector3 dynamicTarget = _movingPlatform.transform.position;
            dynamicTarget.y += platformHeight;

            Vector3 pos = Vector3.Lerp(startPos, dynamicTarget, t);
            pos.y += _archHeight * Mathf.Sin(Mathf.PI * t);

            aiTransform.position = pos;
            aiTransform.rotation = uprightRotation;

            yield return null;
        }

        // Landen op platform
        Vector3 finalPos = _movingPlatform.transform.position;
        finalPos.y += platformHeight;
        aiTransform.position = finalPos;

        patrol.IsJumping = false;

        // NIET inschakelen van agent hier!  
        // De agent blijft uit totdat de AI weer op de grond staat
    }
}