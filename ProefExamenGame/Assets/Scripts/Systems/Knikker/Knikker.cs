using UnityEngine;

public class Knikker : MonoBehaviour
{ 
    #region References

    [Header("References")]
    [SerializeField] private Rigidbody _rb;

    #endregion


    #region Path Settings

    [Header("Path Settings")]
    [SerializeField] private float _moveForce = 80f;
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _reachDistance = 1f;

    #endregion


    #region Knockback Settings

    [Header("Knockback Settings")]
    [SerializeField] private float _force = 12f;
    [SerializeField] private float _upForce = 2.5f;

    #endregion


    #region Detection

    [Header("Detection")]
    [SerializeField] private string _playerTag = "Player";

    #endregion


    #region Lifetime

    [Header("Lifetime")]
    [SerializeField] private float _destroyAfterSeconds = 12f;

    #endregion


    private Transform[] _waypoints;
    private int _currentWaypointIndex;


    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        Destroy(gameObject, _destroyAfterSeconds);
    }


    private void FixedUpdate()
    {
        FollowPath();
    }


    public void SetPath(Transform[] waypoints)
    {
        _waypoints = waypoints;
        _currentWaypointIndex = 0;
    }


    private void FollowPath()
    {
        if (_rb == null)
        {
            return;
        }

        if (_waypoints == null || _waypoints.Length == 0)
        {
            return;
        }

        if (_currentWaypointIndex >= _waypoints.Length)
        {
            return;
        }

        Transform target = _waypoints[_currentWaypointIndex];

        if (target == null)
        {
            _currentWaypointIndex++;
            return;
        }

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        float distanceToTarget = direction.magnitude;

        if (distanceToTarget <= _reachDistance)
        {
            _currentWaypointIndex++;
            return;
        }

        direction = direction.normalized;

        _rb.AddForce(direction * _moveForce, ForceMode.Force);

        Vector3 velocity = _rb.velocity;

        if (velocity.magnitude > _maxSpeed)
        {
            _rb.velocity = velocity.normalized * _maxSpeed;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(_playerTag))
        {
            return;
        }

        KnockbackReceiver receiver = collision.gameObject.GetComponent<KnockbackReceiver>();

        if (receiver == null)
        {
            return;
        }

        Vector3 direction = collision.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            direction = transform.forward;
            direction.y = 0f;
        }

        direction = direction.normalized;

        Vector3 impulse = direction * _force;
        impulse.y = _upForce;

        receiver.ReceiveKnockback(impulse);

        Destroy(gameObject);
    }
}
