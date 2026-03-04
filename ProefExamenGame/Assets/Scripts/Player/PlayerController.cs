using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// <c>PlayerController</c> Controls player movement, rotation, and physics behaviour.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private bool _allowBackwardMovement = true; // New option

    [Header("Ink Spot")]
    [SerializeField] private float _decreasedMovementSpeed = 3f;
    private float _originalMovementSpeed;

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
    private bool _missingAnimationsWarned;
    private bool _missingCameraSettingsWarned;


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
        _originalMovementSpeed = _moveSpeed;
    }

    private void Update()
    {
        // Check for knockback control lock before processing movement input
        if (_knockback != null && _knockback.IsControlLocked)
        {
            SetWalkingAnimation(false);
            return;
        }

        if (_moveInput != Vector2.zero)
        {
            Vector3 moveDir = GetMoveDirection(_moveInput);
            SetWalkingAnimation(true);

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
        else
        {
            SetWalkingAnimation(false);
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
        SetWalkingAnimation(_moveInput != Vector2.zero);
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
            SetJumpingAnimation(false);

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

    private void TryJump()
    {
        if (_knockback != null && _knockback.IsControlLocked) return;
        if (!_isGrounded) return;

        Vector3 velocity = _rb.velocity;
        velocity.y = 0f;
        _rb.velocity = velocity;

        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        SetJumpingAnimation(true);
    }

    private void SetWalkingAnimation(bool isWalking)
    {
        if (_playerAnimations == null)
        {
            if (!_missingAnimationsWarned)
            {
                Debug.LogWarning("PlayerController is missing a PlayerAnimations reference.", this);
                _missingAnimationsWarned = true;
            }

            return;
        }

        _playerAnimations.SetWalking(isWalking);
    }

    private void SetJumpingAnimation(bool isJumping)
    {
        if (_playerAnimations == null)
        {
            if (!_missingAnimationsWarned)
            {
                Debug.LogWarning("PlayerController is missing a PlayerAnimations reference.", this);
                _missingAnimationsWarned = true;
            }

            return;
        }

        _playerAnimations.SetJumping(isJumping);
    }
}
