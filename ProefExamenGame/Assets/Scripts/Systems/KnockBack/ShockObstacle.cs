using UnityEngine;

public class ShockObstacle : MonoBehaviour
{
    [Header("References (optional)")]
    [SerializeField] private PlayerKnockback _specificTarget; // sleep hier player knockback in als je wil

    [Header("Hit Settings")]
    [SerializeField] private float _hitCooldown = 0.5f;

    private float _lastHitTime;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < _lastHitTime + _hitCooldown) return;

        PlayerKnockback knock = _specificTarget;

        if (knock == null)
        {
            if (!other.CompareTag("Player")) return;
            knock = other.GetComponent<PlayerKnockback>();
        }

        if (knock == null) return;

        _lastHitTime = Time.time;
        knock.ApplyKnockback(transform.position);
    }
}
