using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private bool allowBackwardMovement = true; // New option

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Upright Settings")]
    [SerializeField] private float springStrength = 200f;
    [SerializeField] private float springDamping = 25f;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector3 lastForwardDirection; // Track last forward direction

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        lastForwardDirection = transform.forward;
    }

    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            Vector3 moveDir = GetCameraRelativeMovement(_moveInput);
            
            if (moveDir != Vector3.zero)
            {
                // Only rotate if moving forward or sideways (not backward)
                float inputMagnitude = _moveInput.magnitude;
                float forwardInput = _moveInput.y;
                
                // If moving forward or mostly sideways, update rotation
                if (forwardInput >= -0.5f) // Allow slight backward without spinning
                {
                    lastForwardDirection = moveDir.normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(lastForwardDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                }
                // If moving backward, keep current rotation
                else if (allowBackwardMovement)
                {
                    // Maintain current forward direction
                    Quaternion targetRotation = Quaternion.LookRotation(lastForwardDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyUprightTorque();
    }

    private void ApplyMovement()
    {
        float currentYVelocity = _rb.velocity.y;
        
        Vector3 moveDirection = GetCameraRelativeMovement(_moveInput);
        Vector3 newVelocity = moveDirection * _moveSpeed;
        
        newVelocity.y = currentYVelocity;
        
        _rb.velocity = newVelocity;
    }

    private Vector3 GetCameraRelativeMovement(Vector2 input)
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        Vector3 moveDirection = (cameraRight * input.x) + (cameraForward * input.y);
        
        return moveDirection;
    }

    public void SetMovementInput(Vector2 input)
    {
        _moveInput = input;
    }

    public void OnJumpButtonPressed()
    {
        if (_isGrounded)
        {
            Vector3 vel = _rb.velocity;
            vel.y = 0;
            _rb.velocity = vel;
            
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded && context.performed)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = false;
        }
    }

    private void ApplyUprightTorque()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        Quaternion delta = targetRotation * Quaternion.Inverse(transform.rotation);

        if (delta.w < 0f)
        {
            delta.x = -delta.x;
            delta.y = -delta.y;
            delta.z = -delta.z;
            delta.w = -delta.w;
        }

        Vector3 rotationVector = new Vector3(delta.x, delta.y, delta.z);
        Vector3 torque = (2f * rotationVector * springStrength) - (_rb.angularVelocity * springDamping);
        
        _rb.AddTorque(torque, ForceMode.Acceleration);
    }
}