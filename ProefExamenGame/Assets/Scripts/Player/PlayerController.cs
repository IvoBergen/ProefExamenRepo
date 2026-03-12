using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// <c>PlayerController</c> Controls player movement, rotation, and physics behaviour.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isDashing;
    [SerializeField] private bool _isGettingUp;
    [SerializeField] private bool _isDead;
    public float _moveSpeed { get; set; } = 5f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _doubleJumpForce;
    [SerializeField] private int _maxJumpCount = 2;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private bool _allowBackwardMovement = true; // New option
    [SerializeField] private float _movementDirectionThreshold = 0.2f;

    [Header("Ink Spot")]
    [SerializeField] private float _decreasedMovementSpeed = 3f;
    public float _originalMovementSpeed { get; private set; }

    [Header("Upright Settings")]
    [SerializeField] private float _springStrength = 200f;
    [SerializeField] private float _springDamping = 25f;

    [Header("References")]
    [SerializeField] private KnockbackReceiver _knockback;
    [SerializeField] private PlayerAnimations _playerAnimations;
    [SerializeField] private CameraSettings _cameraSettings;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector3 lastForwardDirection; // Track last forward direction
    private Vector3 _stablePlanarMovementDirection;
    private int _jumpsUsed;
    private bool _missingAnimationsWarned;
    private bool _missingCameraSettingsWarned;

    public Vector3 StablePlanarMovementDirection { get; private set; }
    public float PlanarMovementMagnitude { get; private set; }


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

        lastForwardDirection = transform.forward;
        _stablePlanarMovementDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        StablePlanarMovementDirection = _stablePlanarMovementDirection;
        _originalMovementSpeed = _moveSpeed;
        _doubleJumpForce = _jumpForce * 0.85f;
    }

    private void Update()
    {
        // Check for knockback control lock before processing movement input
        if (_knockback != null && _knockback.IsControlLocked)
        {
            PlanarMovementMagnitude = 0f;
            UpdateAnimationStates();
            return;
        }

        Vector3 moveDir = GetMoveDirection(_moveInput);
        UpdateStableMovementDirection(moveDir);

        if (_moveInput != Vector2.zero)
        {
            if (moveDir != Vector3.zero)
            {
                // Only rotate if moving forward or sideways (not backward)
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
        UpdateAnimationStates();
    }

    private void FixedUpdate()
    {
        // reset jumps when grounded
        if (_isGrounded)
        {
            _jumpsUsed = 0;
        }

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

        Vector3 moveDirection = GetMoveDirection(_moveInput);
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

    public void SetMovementInput(Vector2 input)
    {
        _moveInput = input;
    }

    public void OnJumpButtonPressed()
    {
        TryJump();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TryJump();
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

    private Vector3 GetMoveDirection(Vector2 input)
    {
        if (_cameraSettings == null)
        {
            if (!_missingCameraSettingsWarned)
            {
                Debug.LogWarning("PlayerController is missing a CameraSettings reference.", this);
                _missingCameraSettingsWarned = true;
            }

            return Vector3.zero;
        }

        return _cameraSettings.GetCameraRelativeMovement(input);
    }

    /// <summary>
    /// Keeps the last stable planar movement direction available for other systems.
    /// </summary>
    private void UpdateStableMovementDirection(Vector3 moveDirection)
    {
        moveDirection.y = 0f;
        PlanarMovementMagnitude = Mathf.Clamp01(moveDirection.magnitude);

        if (PlanarMovementMagnitude >= _movementDirectionThreshold)
        {
            _stablePlanarMovementDirection = moveDirection.normalized;
        }

        StablePlanarMovementDirection = _stablePlanarMovementDirection;
    }

    private void TryJump()
    {
        if (_knockback != null && _knockback.IsControlLocked) return;
        if (_jumpsUsed >= _maxJumpCount) return;
        if (_jumpsUsed == 0 && !_isGrounded) return;

        Vector3 velocity = _rb.velocity;
        velocity.y = 0f;
        _rb.velocity = velocity;

        float jumpForce = _jumpsUsed == 0 ? _jumpForce : _doubleJumpForce;
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _isGrounded = false;
        _jumpsUsed++;
    }

    public void SetDashingState(bool isDashing)
    {
        _isDashing = isDashing;
    }

    public void SetDeadState(bool isDead)
    {
        _isDead = isDead;
    }

    public void SetGettingUpState(bool isGettingUp)
    {
        _isGettingUp = isGettingUp;
    }

    private void UpdateAnimationStates()
    {
        if (!TryGetAnimations(out PlayerAnimations animations))
        {
            return;
        }

        bool hasMoveInput = _moveInput.sqrMagnitude > 0.0001f;
        bool isDead = _isDead;
        bool isGettingUp = _isGettingUp && !isDead;
        bool isDashing = _isDashing && !isDead && !isGettingUp;
        bool isAirborne = !_isGrounded;
        float verticalVelocity = _rb != null ? _rb.velocity.y : 0f;

        bool isJumping = isAirborne && verticalVelocity > 0.01f && !isDead && !isGettingUp && !isDashing;
        bool isFalling = isAirborne && verticalVelocity < -0.01f && !isDead && !isGettingUp && !isDashing;
        bool isWalking = hasMoveInput && _isGrounded && !isDead && !isGettingUp && !isDashing;
        bool isIdle = _isGrounded && !hasMoveInput && !isDead && !isGettingUp && !isDashing;

        animations.SetIdle(isIdle);
        animations.SetWalking(isWalking);
        animations.SetDashing(isDashing);
        animations.SetJumping(isJumping);
        animations.SetFalling(isFalling);
        animations.SetDeath(isDead);
        animations.SetGettingUp(isGettingUp);
    }

    private bool TryGetAnimations(out PlayerAnimations animations)
    {
        animations = _playerAnimations;
        if (animations != null)
        {
            return true;
        }

        if (!_missingAnimationsWarned)
        {
            Debug.LogWarning("PlayerController is missing a PlayerAnimations reference.", this);
            _missingAnimationsWarned = true;
        }

        return false;
    }
}
    
