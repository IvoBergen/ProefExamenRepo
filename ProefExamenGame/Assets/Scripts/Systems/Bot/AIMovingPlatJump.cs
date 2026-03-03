using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMovingPlatJump : MonoBehaviour
{
    public MovingCubes movingPlatform;

    public float jumpDuration = 0.8f;
    public float archHeight = 2f;
    public float waitOnPlatform = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (movingPlatform == null) return;
        if (!other.CompareTag("Bot")) return;

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        AIMoveNavMesh patrol = other.GetComponent<AIMoveNavMesh>();
        if (patrol == null) return;

        StartCoroutine(JumpToPlatform(other.transform, agent, patrol));
    }

    private IEnumerator JumpToPlatform(
        Transform aiTransform,
        NavMeshAgent agent,
        AIMoveNavMesh patrol)
    {
        patrol.IsJumping = true;

        // 🛑 Pause agent safely
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;

        Vector3 startPos = aiTransform.position;

        Vector3 targetPos = movingPlatform.transform.position;

        Renderer rend = movingPlatform.GetComponent<Renderer>();
        float platformHeight = rend != null
            ? rend.bounds.size.y
            : movingPlatform.transform.localScale.y;

        targetPos.y += platformHeight;

        Quaternion uprightRotation =
            Quaternion.Euler(0f, aiTransform.eulerAngles.y, 0f);

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += archHeight * Mathf.Sin(Mathf.PI * t);

            aiTransform.position = currentPos;
            aiTransform.rotation = uprightRotation;

            yield return null;
        }

        aiTransform.position = targetPos;
        aiTransform.rotation = uprightRotation;

        yield return new WaitForSeconds(waitOnPlatform);

        // 🔄 Re-sync agent to new position
        agent.Warp(aiTransform.position);

        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        patrol.IsJumping = false;
    }
}