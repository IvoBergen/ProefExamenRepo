using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BotPathRandomizer : MonoBehaviour
{
    private NavMeshAgent _agent;

    [Header("Speed Personality")]
    public float minSpeed = 3.5f;
    public float maxSpeed = 6f;

    [Header("Path Randomness")]
    public float pathOffsetStrength = 1.8f;   // how far they deviate from path
    public float waypointChangeInterval = 1.2f;

    [Header("Human Mistakes")]
    public float hesitationChance = 0.15f;
    public float hesitationTime = 0.6f;

    private Vector3 _currentTarget;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Give each bot unique movement stats
        _agent.speed = Random.Range(minSpeed, maxSpeed);
        _agent.acceleration *= Random.Range(0.8f, 1.3f);
        _agent.angularSpeed *= Random.Range(0.7f, 1.4f);

        StartCoroutine(RandomizePathRoutine());
    }

    IEnumerator RandomizePathRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waypointChangeInterval);

            if (!_agent.hasPath) continue;

            // Random hesitation (human behaviour)
            if (Random.value < hesitationChance)
            {
                _agent.isStopped = true;
                yield return new WaitForSeconds(Random.Range(0.2f, hesitationTime));
                _agent.isStopped = false;
            }

            // Add sideways drift to path
            Vector3 dir = _agent.desiredVelocity.normalized;

            Vector3 sideways = Vector3.Cross(Vector3.up, dir);
            float offsetAmount = Random.Range(-pathOffsetStrength, pathOffsetStrength);

            Vector3 offset = sideways * offsetAmount;
            Vector3 newDestination = _agent.destination + offset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(newDestination, out hit, 2f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }
}
