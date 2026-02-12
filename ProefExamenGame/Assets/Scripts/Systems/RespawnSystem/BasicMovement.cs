using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // snelheid van de bean
    private Rigidbody rb;

    private Vector3 moveInput;
    private Vector3 moveVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Input van toetsenbord ophalen
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D of pijltjes links/rechts
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S of pijltjes omhoog/omlaag

        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
        moveVelocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        // Beweging toepassen via Rigidbody
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
}
