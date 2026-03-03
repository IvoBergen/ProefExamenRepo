using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// <c>PlayerController</c> Controls the movement + camera + Animations of the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private bool _isGrounded;
    public float _moveSpeed { get; set; } = 5f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private bool _allowBackwardMovement = true; // New option

    [Header("Ink Spot")]
    [SerializeField] private float _decreasedMovementSpeed = 3f;
    public float _originalMovementSpeed { get; private set; }

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Upright Settings")]
    [SerializeField] private float _springStrength = 200f;
    [SerializeField] private float _springDamping = 25f;

    [Header("References")]
    [SerializeField] private KnockbackReceiver _knockback;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector3 lastForwardDirection; // Track last forward direction


    private void OnEnable()
    {
        InkSpot.onInkEntered += DecreaseMovementSpeed;
        InkSpot.onInkExited += ResetMovementSpeed;
    }

    private void OnDisable()
    {
        InkSpot.onInkEntered -= DecreaseMovementSpeed;
        InkSpot.onInkExited -= ResetMovementSpeed;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform;
        }

        lastForwardDirection = transform.forward;
        _originalMovementSpeed = _moveSpeed;
    }

    private void Update()
    {
        // Check for knockback control lock before processing movement input
        if (_knockback != null && _knockback.IsControlLocked)
        {
            _animator.SetBool("isWalking", false);
            return;
        }

        if (_moveInput != Vector2.zero)
        {
            Vector3 moveDir = GetCameraRelativeMovement(_moveInput);
            _animator.SetBool("isWalking", true);

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
                else if (_allowBackwardMovement)
                {
                    // Maintain current forward direction
                    Quaternion targetRotation = Quaternion.LookRotation(lastForwardDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }
    }

    private void FixedUpdate()
    {
        // Only apply movement if not under knockback control lock
        if (_knockback == null || !_knockback.IsControlLocked)
        {
            ApplyMovement();
        }

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

    private void DecreaseMovementSpeed()
    {
        _moveSpeed = _decreasedMovementSpeed;
    }

    private void ResetMovementSpeed()
    {
        _moveSpeed = _originalMovementSpeed;
    }

    private Vector3 GetCameraRelativeMovement(Vector2 input)
    {
        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

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

            _animator.SetBool("isJumping", true);
        }
        else
        {
            _animator.SetBool("isJumping", false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        _animator.SetBool("isWalking", _moveInput != Vector2.zero);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Check for knockback control lock before allowing jump
        if (!context.performed) return;
        if (!_isGrounded) return;

        Vector3 velocity = _rb.velocity;
        velocity.y = 0f;
        _rb.velocity = velocity;

        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _animator.SetBool("isJumping", true);
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
        Vector3 torque = (2f * rotationVector * _springStrength) - (_rb.angularVelocity * _springDamping);

        _rb.AddTorque(torque, ForceMode.Acceleration);
    }
}