using UnityEngine;

/// <summary>
/// <c>MovingCubes</c> moves the cube in a random direction and when it reaches the target position, 
/// it generates a new random position within the specified boundaries and moves towards it.
/// </summary>
public class MovingCubes : MonoBehaviour
{
    [Header("Movement Boundaries")]
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;

    private Vector3 _targetposition;

    [Header("Movement Settings")]
    public float speed;


    private void Start()
    {
        _targetposition = RandomPosition();
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    /// <summary>
    /// <c>RandomPosition</c> generates a random position within the specified boundaries for the cube to move towards.
    /// </summary>
    /// <returns>The random position</returns>
    private Vector3 RandomPosition()
    {
        float randomX = Random.Range(_minX, _maxX);
        float randomZ = Random.Range(_minZ, _maxZ);
        return new Vector3(randomX, transform.position.y, randomZ);
    }

    /// <summary>
    /// <c>MoveTowardsTarget</c> Moves the cube to the given target position 
    /// and checks if the cube is close enough to the target position to generate a new random position.
    /// </summary>
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetposition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetposition) < 0.1f)
        {
            _targetposition = RandomPosition();
        }
    }

}
