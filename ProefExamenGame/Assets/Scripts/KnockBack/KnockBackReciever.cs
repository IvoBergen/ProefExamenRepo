using UnityEngine;

/// <summary>
/// Receives knockback impulses from obstacles and temporarily locks player control.
/// Also clamps horizontal velocity to prevent excessive launch speeds.
/// </summary>
public class KnockbackReceiver : MonoBehaviour
{
    #region References

    [Header("References")]

    [SerializeField] private Rigidbody _rb;

    #endregion


    #region Lock Timings

    [Header("Lock Timings")]

    [SerializeField] private float _controlLockDuration = 1.0f;

    #endregion


    #region Clamp Settings

    [Header("Clamp After Hit")]

    [SerializeField] private float _maxHorizontalSpeedAfterHit = 10f;

    #endregion


    private float _controlLockUntil;
    public bool IsControlLocked => Time.time < _controlLockUntil;


    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Applies a knockback impulse to the Rigidbody and locks player control
    /// for a short duration.
    /// </summary>
    public void ReceiveKnockback(Vector3 impulse)
    {
        _controlLockUntil = Time.time + _controlLockDuration;

        _rb.AddForce(impulse, ForceMode.Impulse);

        ClampHorizontalVelocity();
    }


    /// <summary>
    /// Limits horizontal velocity after a knockback to prevent unrealistic launches.
    /// Vertical velocity remains untouched.
    /// </summary>
    private void ClampHorizontalVelocity()
    {
        if (_maxHorizontalSpeedAfterHit <= 0f)
        {
            return;
        }

        Vector3 velocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontalVelocity.magnitude <= _maxHorizontalSpeedAfterHit)
        {
            return;
        }

        horizontalVelocity = horizontalVelocity.normalized * _maxHorizontalSpeedAfterHit;

        _rb.velocity = new Vector3(
            horizontalVelocity.x,
            velocity.y,
            horizontalVelocity.z
        );
    }
}
