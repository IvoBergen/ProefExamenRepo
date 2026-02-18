using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KnockbackObstacle : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float _force = 12f;
    [SerializeField] private float _upForce = 2.5f;

    [Header("Timing")]
    [SerializeField] private float _cooldown = 0.5f;

    [Header("Clamp")]
    [SerializeField] private float _maxHorizontalSpeedAfterHit = 10f;

    [Header("Detection")]
    [SerializeField] private string _playerTag = "Player";

    private float _lastHitTime;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag))
            return;

        if (Time.time < _lastHitTime + _cooldown)
            return;

        _lastHitTime = Time.time;

        Rigidbody rb = GetRigidbody(other);
        if (rb == null)
            return;

        ApplyKnockback(rb);
        ClampHorizontalVelocity(rb);
    }

    private Rigidbody GetRigidbody(Collider other)
    {
        return other.attachedRigidbody != null 
            ? other.attachedRigidbody 
            : other.GetComponent<Rigidbody>();
    }

    private void ApplyKnockback(Rigidbody rb)
    {
        Vector3 direction = rb.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            direction = -transform.forward;

        direction.Normalize();

        // Push away from obstacle + small lift
        Vector3 forceVector = direction * _force + Vector3.up * _upForce;
        rb.AddForce(forceVector, ForceMode.Impulse);
    }

    private void ClampHorizontalVelocity(Rigidbody rb)
    {
        if (_maxHorizontalSpeedAfterHit <= 0f)
            return;

        Vector3 velocity = rb.velocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontal.magnitude <= _maxHorizontalSpeedAfterHit)
            return;

        horizontal = horizontal.normalized * _maxHorizontalSpeedAfterHit;
        rb.velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }
}
