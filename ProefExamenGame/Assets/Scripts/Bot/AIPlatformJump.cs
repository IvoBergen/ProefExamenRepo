using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlatformJump : MonoBehaviour
{
    [Header("Target Platform")]
    public Transform targetPlatform;

    [Header("Platform Type Settings")]
    public bool useParenting = true;

    [Header("Jump Settings")]
    public float jumpDuration = 1.2f;
    public float jumpHeight = 1.5f;
    public float jumpDelay = 0.2f;

    [Header("Fail Settings")]
    [Range(0f, 1f)]
    public float failChance = 0.15f;
    public float failRespawnDelay = 5f;
    public float failForwardLoss = 0.4f;

    private HashSet<Transform> jumpingBots = new HashSet<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bot") || targetPlatform == null) return;
        if (jumpingBots.Contains(other.transform)) return;

        Transform bot = other.transform;

        NavMeshAgent agent = bot.GetComponent<NavMeshAgent>();
        Rigidbody rb = bot.GetComponent<Rigidbody>();
        CapsuleCollider cap = bot.GetComponent<CapsuleCollider>();
        AIMoveNavMesh patrol = bot.GetComponent<AIMoveNavMesh>();

        if (agent == null || rb == null || cap == null) return;

        jumpingBots.Add(bot);
        StartCoroutine(JumpRoutine(bot, agent, rb, cap, patrol));
    }

    private IEnumerator JumpRoutine(Transform ai, NavMeshAgent agent, Rigidbody rb, CapsuleCollider cap, AIMoveNavMesh patrol)
    {
        yield return new WaitForSeconds(jumpDelay);

        Vector3 lookDir = targetPlatform.position - ai.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            ai.rotation = Quaternion.LookRotation(lookDir);

        ai.SetParent(null);

        if (agent.enabled)
            agent.enabled = false;

        rb.isKinematic = true;

        Vector3 startPos = ai.position;
        Vector3 targetPos = targetPlatform.position;

        float elapsed = 0f;
        bool failed = Random.value < failChance;

        float botHalfHeight = cap.bounds.extents.y * 0.05f;
        targetPos.y = GetPlatformTopY() + botHalfHeight;

        Vector3 lastPos = startPos;
        Vector3 velocity = Vector3.zero;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += Mathf.Sin(Mathf.PI * t) * jumpHeight;

            velocity = (pos - lastPos) / Time.deltaTime;
            lastPos = pos;

            ai.position = pos;

            if (failed && t > 0.55f)
                break;

            yield return null;
        }

        if (failed)
        {
            cap.enabled = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            velocity *= failForwardLoss;

            rb.velocity = velocity;

            ai.Rotate(Vector3.forward * Random.Range(-15f, 15f));

            yield return new WaitForSeconds(failRespawnDelay);

            if (CheckPointManager.Instance != null && CheckPointManager.Instance.CurrentCheckpoint != null)
            {
                Transform respawn = CheckPointManager.Instance.CurrentCheckpoint.transform;

                ai.position = respawn.position;

                agent.Warp(respawn.position);
                agent.enabled = true;

                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;

                cap.enabled = true;

                if (patrol != null)
                    patrol.ResetPatrol();
            }
        }
        else
        {
            ai.position = targetPos;

            if (useParenting)
            {
                ai.SetParent(targetPlatform);
                rb.isKinematic = true;
            }
            else
            {
                ai.SetParent(null);
                rb.isKinematic = false;
            }
        }

        jumpingBots.Remove(ai);
    }

    private float GetPlatformTopY()
    {
        Renderer r = targetPlatform.GetComponent<Renderer>();
        if (r != null)
            return r.bounds.max.y;

        return targetPlatform.position.y + (targetPlatform.localScale.y * 0.5f);
    }
}