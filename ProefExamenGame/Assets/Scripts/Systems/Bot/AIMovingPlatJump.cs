using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMovingPlatJump : MonoBehaviour
{
    public MovingCubes movingPlatform;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("player touched me");
        if (movingPlatform == null) return;
        if (!other.CompareTag("Bot")) return;

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        StartCoroutine(JumpToPlatform(other.transform, agent));
    }

    private IEnumerator JumpToPlatform(Transform aiTransform, NavMeshAgent agent)
    {
        agent.enabled = false;

        Vector3 startPos = aiTransform.position;

        Vector3 targetPos = movingPlatform.transform.position;
        float platformHeight = 0f;

        Renderer rend = movingPlatform.GetComponent<Renderer>();
        if (rend != null)
            platformHeight = rend.bounds.size.y;
        else
            platformHeight = movingPlatform.transform.localScale.y;

        targetPos.y += platformHeight;

        float jumpDuration = 0.6f;
        float elapsed = 0f;
        float archHeight = 2f;

        Quaternion uprightRotation = Quaternion.Euler(0f, aiTransform.eulerAngles.y, 0f); // lock X/Z rotation

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += archHeight * Mathf.Sin(Mathf.PI * t);

            aiTransform.position = currentPos;
            aiTransform.rotation = uprightRotation; // keep upright

            yield return null;
        }

        aiTransform.position = targetPos;
        aiTransform.rotation = uprightRotation;
        aiTransform.SetParent(movingPlatform.transform);

        yield return new WaitForSeconds(2f);

        aiTransform.SetParent(null);
        agent.enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        // Safety unparent if something slips through
        if (other.transform.parent == movingPlatform.transform)
        {
            other.transform.SetParent(null);
        }
    }
}
