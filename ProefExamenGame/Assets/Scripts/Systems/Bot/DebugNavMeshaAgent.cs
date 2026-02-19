using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// used for debugging the path of the AI 
/// </summary>
public class DebugNavMeshaAent : MonoBehaviour
{
    public bool velocity;
    public bool desiredvelocity;
    public bool path;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (velocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }
        if (desiredvelocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
        }
        if (path)
        {
            Gizmos.color = Color.black;
            var agentpath = agent.path;
            Vector3 prevcorner = transform.position;
            foreach (var corner in agentpath.corners)
            {
                Gizmos.DrawLine(prevcorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                prevcorner = corner;
            }
        }
    }
}
