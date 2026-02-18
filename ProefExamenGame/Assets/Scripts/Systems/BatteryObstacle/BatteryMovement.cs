using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BatteryMovement : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        /// Get required Rigidbody component
        _rigidbody = GetComponent<Rigidbody>();

        /// Ensure this object is controlled manually via physics
        _rigidbody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        RotateObstacle();
    }

    /// <summary>
    /// Rotates the obstacle using Rigidbody physics
    /// </summary>
    private void RotateObstacle()
    {
        Quaternion deltaRotation = Quaternion.Euler( 
            _rotationAxis * _rotationSpeed * Time.fixedDeltaTime
        );

        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }
}
