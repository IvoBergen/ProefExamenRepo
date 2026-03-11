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
    [SerializeField] private string _botTag = "Bot";

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


    /// <summary>
    /// Sets the waypoint path that the ball should follow.
    /// </summary>
    public void SetPath(Transform[] waypoints)
    {
        _waypoints = waypoints;
        _currentWaypointIndex = 0;
    }


    /// <summary>
    /// Moves the ball toward the current waypoint.
    /// </summary>
    private void FollowPath()
    {
        if (_rb == null) return;
        if (_waypoints == null || _waypoints.Length == 0) return;
        if (_currentWaypointIndex >= _waypoints.Length) return;

        Transform currentTarget = _waypoints[_currentWaypointIndex];

        if (currentTarget == null)
        {
            _currentWaypointIndex++;
            return;
        }

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        float distanceToTarget = direction.magnitude;

        if (distanceToTarget <= _reachDistance)
        {
            _currentWaypointIndex++;
            return;
        }

        direction.Normalize();

        _rb.AddForce(direction * _moveForce, ForceMode.Force);

        ClampSpeed();
    }


    /// <summary>
    /// Limits the ball velocity.
    /// </summary>
    private void ClampSpeed()
    {
        Vector3 velocity = _rb.velocity;

        if (velocity.magnitude <= _maxSpeed)
        {
            return;
        }

        _rb.velocity = velocity.normalized * _maxSpeed;
    }


    /// <summary>
    /// Handles collision with Player or Bot.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(_playerTag) &&
            !collision.gameObject.CompareTag(_botTag))
        {
            return;
        }

        KnockbackReceiver knockbackReceiver = collision.gameObject.GetComponent<KnockbackReceiver>();

        if (knockbackReceiver == null)
        {
            return;
        }

        ApplyKnockback(knockbackReceiver, collision.transform);
        Destroy(gameObject);
    }


    /// <summary>
    /// Calculates knockback impulse.
    /// </summary>
    private void ApplyKnockback(KnockbackReceiver receiver, Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            direction = transform.forward;
            direction.y = 0f;
        }

        direction.Normalize();

        Vector3 impulse = direction * _force;
        impulse.y = _upForce;

        receiver.ReceiveKnockback(impulse);
    }
}