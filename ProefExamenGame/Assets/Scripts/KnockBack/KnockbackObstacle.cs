using UnityEngine;

/// <summary>
/// Applies a knockback impulse to the player when entering this trigger.
/// This obstacle is used in the obstacle course level to push the player away.
/// </summary>
[RequireComponent(typeof(Collider))]
public class KnockbackObstacle : MonoBehaviour
{
    #region Knockback Settings

    [Header("Knockback Settings")]

    [SerializeField] private float _force = 12f;
    [SerializeField] private float _upForce = 2.5f;

    #endregion


    #region Timing

    [Header("Timing")]

    [SerializeField] private float _cooldown = 0.5f;

    #endregion


    #region Detection

    [Header("Detection")]

    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private string _AiTag = "Bot";

    #endregion


    private float _lastHitTime;


    /// <summary>
    /// Automatically sets the collider to trigger mode when the component is added.
    /// This ensures the obstacle works correctly with OnTriggerEnter.
    /// </summary>
    private void Reset()
    {
        Collider colliderComponent = GetComponent<Collider>();
        colliderComponent.isTrigger = true;
    }


    /// <summary>
    /// Detects when an object enters the trigger.
    /// If the object is the player and the cooldown has passed,
    /// a knockback impulse will be applied.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag) && !other.CompareTag(_AiTag))
        {
            return;
        }

        if (Time.time < _lastHitTime + _cooldown)
        {
            return;
        }

        _lastHitTime = Time.time;

        KnockbackReceiver knockbackReceiver = other.GetComponentInParent<KnockbackReceiver>();

        if (knockbackReceiver == null)
        {
            return;
        }

        ApplyKnockback(knockbackReceiver);
        Debug.LogWarning("bothit");
    }


    /// <summary>
    /// Calculates the knockback direction and applies the impulse to the player.
    /// </summary>
    /// <param name="receiver">The knockback receiver component on the player.</param>
    private void ApplyKnockback(KnockbackReceiver receiver)
    {
        Vector3 direction = receiver.transform.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        Vector3 impulse = direction * _force + Vector3.up * _upForce;

        receiver.ReceiveKnockback(impulse);
    }
}
