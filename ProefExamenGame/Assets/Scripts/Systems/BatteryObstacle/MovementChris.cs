using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementChris : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _acceleration = 20f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 6f;

    [Header("Tags")]
    [SerializeField] private string _groundTag = "Ground";

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private bool _jumpPressed;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();

        _jumpPressed = false;
    }

    private void ReadInput()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            _jumpPressed = true;
    }

    private void HandleMovement()
    {
        Vector3 wishDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        wishDirection = Vector3.ClampMagnitude(wishDirection, 1f);

        Vector3 currentVelocity = _rb.velocity;
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 targetHorizontal = wishDirection * _moveSpeed;

        Vector3 newHorizontal = Vector3.MoveTowards(
            currentHorizontal,
            targetHorizontal,
            _acceleration * Time.fixedDeltaTime
        );

        _rb.velocity = new Vector3(newHorizontal.x, currentVelocity.y, newHorizontal.z);
    }

    private void HandleJump()
    {
        if (!_jumpPressed || !_isGrounded)
            return;

        // Reset vertical velocity for consistent jump height
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(_groundTag))
            _isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(_groundTag))
            _isGrounded = false;
    }
}
