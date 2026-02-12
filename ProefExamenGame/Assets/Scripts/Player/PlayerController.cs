using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]

    [SerializeField, Tooltip("Is the player currently grounded?")] private bool _isGrounded;
    [SerializeField, Tooltip("Movement speed of the player")] private float _moveSpeed;
    [SerializeField, Tooltip("Jump force applied to the player")] private float _jumpForce;
    [SerializeField, Tooltip("Rotation speed when turning towards movement direction")] private float _rotationSpeed = 10f;
    [SerializeField, Tooltip("Gravity multiplier for better control")] private float _gravityScale = 1f;

    [Header("Upright Settings")]
    
    [SerializeField, Tooltip("Spring strength for upright torque")] private float springStrength = 200f;
    [SerializeField, Tooltip("Spring damping for upright torque")] private float springDamping = 25f;

    private Rigidbody _rb;

    private Vector3 _moveDirection;
    private Vector3 localMoveDirection;
    private Vector3 _currentVelocity = Vector3.zero;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_moveDirection != Vector3.zero)
        {
            // Rotate towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(localMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        ApplyUprightTorque();
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        // Read movement input (WASD or other)
        _moveDirection = value.ReadValue<Vector3>().normalized;

        // Preserve the current vertical velocity (Y) from the Rigidbody
        float currentYVelocity = _rb.velocity.y;

        // Apply movement to X and Z axes with a smooth diagonal movement
        localMoveDirection = transform.TransformDirection(_moveDirection); // Convert input to local space
        Vector3 horizontalVelocity = new Vector3(localMoveDirection.x, 0f, localMoveDirection.z) * _moveSpeed;

        // If not grounded, apply gravity manually with a custom fall speed multiplier
        if (!_isGrounded)
        {
            // Apply gravity with the fall speed modifier for more control over fall speed
            horizontalVelocity.y = currentYVelocity + (Physics.gravity.y * _gravityScale * Time.deltaTime);
        }
        else
        {
            // Maintain original Y velocity if grounded
            horizontalVelocity.y = currentYVelocity;
        }

        // Apply the velocity with smooth diagonal movement
        _rb.velocity = horizontalVelocity;
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        // Allow jumping only if grounded and the jump button is pressed
        if (_isGrounded && value.performed)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3 && !_isGrounded) // Layer 3 is set as Ground Type
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 3 && _isGrounded) // Layer 3 is set as Ground Type
        {
            _isGrounded = false;
        }
    }

    private void ApplyUprightTorque()
    {
        // Desired upright rotation (world up)
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        // Rotation from current → target
        Quaternion delta = targetRotation * Quaternion.Inverse(transform.rotation);

        // Ensure shortest hemisphere
        if (delta.w < 0f)
        {
            delta.x = -delta.x;
            delta.y = -delta.y;
            delta.z = -delta.z;
            delta.w = -delta.w;
        }

        // Imaginary part gives axis * sin(theta/2)
        Vector3 rotationVector = new Vector3(delta.x, delta.y, delta.z);

        // PD controller torque
        Vector3 torque =
            (2f * rotationVector * springStrength) -
            (_rb.angularVelocity * springDamping);

        _rb.AddTorque(torque, ForceMode.Acceleration);
    }
}