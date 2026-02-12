using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [Header("References (drag in Inspector)")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MovementChris _movement; // sleep je movement script hier in

    [Header("Knockback Feel")]
    [SerializeField] private float _knockbackForce = 12f;
    [SerializeField] private float _knockbackUpForce = 2.5f;

    [Tooltip("Hoe lang de echte 'push' state duurt (kort).")]
    [SerializeField] private float _pushDuration = 0.2f;

    [Tooltip("Hoe lang input geblokkeerd is (1-2 sec voor jouw wens).")]
    [SerializeField] private float _controlLockDuration = 1.5f;

    [Tooltip("Clamp horizontale snelheid na hit (voorkomt mega launch).")]
    [SerializeField] private float _maxHorizontalSpeedAfterHit = 10f;

    [Header("Debug")]
    [SerializeField] private bool _showDebug = false;

    private float _pushTimer;
    private float _controlLockTimer;

    public bool IsPushing => _pushTimer > 0f;
    public bool ControlsLocked => _controlLockTimer > 0f;

    private void Reset()
    {
        // Handig: auto invullen als je op Reset klikt in Inspector
        _rb = GetComponent<Rigidbody>();
        _movement = GetComponent<MovementChris>();
    }

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_movement == null) _movement = GetComponent<MovementChris>();
    }

    private void Update()
    {
        // Timers aftellen
        if (_pushTimer > 0f) _pushTimer -= Time.deltaTime;
        if (_controlLockTimer > 0f) _controlLockTimer -= Time.deltaTime;

        // Als je movement script een setter heeft (optioneel), kun je hier syncen
        // Maar meestal checkt MovementChris gewoon ControlsLocked.
    }

    public void ApplyKnockback(Vector3 sourcePosition)
    {
        // Richting weg van het obstakel (alleen XZ zodat het “over de grond” voelt)
        Vector3 dir = transform.position - sourcePosition;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            dir = -transform.forward;

        dir.Normalize();

        // Timers starten
        _pushTimer = _pushDuration;
        _controlLockTimer = _controlLockDuration;

        // Impulse toevoegen (geen velocity hard reset -> minder “teleport”)
        Vector3 force = dir * _knockbackForce + Vector3.up * _knockbackUpForce;
        _rb.AddForce(force, ForceMode.Impulse);

        // Clamp horizontale snelheid (optioneel maar voelt vaak beter)
        Vector3 v = _rb.velocity;
        Vector3 horizontal = new Vector3(v.x, 0f, v.z);

        if (horizontal.magnitude > _maxHorizontalSpeedAfterHit)
        {
            horizontal = horizontal.normalized * _maxHorizontalSpeedAfterHit;
            _rb.velocity = new Vector3(horizontal.x, v.y, horizontal.z);
        }

        if (_showDebug)
            Debug.Log($"Knockback: dir={dir}, locked={_controlLockDuration}s");
    }
}
