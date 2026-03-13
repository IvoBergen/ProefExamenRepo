using UnityEngine;

/// <summary>
/// <c>CameraCollider</c> resolves and smooths camera collision against world geometry.
/// </summary>
public class CameraCollider : MonoBehaviour
{
    [Header("Camera Collision")]
    [SerializeField] private LayerMask _cameraCollisionMask = ~0;
    [SerializeField] private float _collisionSphereRadius = 0.25f;
    [SerializeField] private float _collisionWallPadding = 0.15f;
    [SerializeField] private float _collisionMinDistance = 1f;
    [SerializeField] private float _collisionRayHeight = 1.2f;
    [SerializeField] private float _collisionMaxDrop = 2f;
    [SerializeField] private float _collisionPositionSharpness = 14f;

    private bool _hasSmoothedCollisionPosition;
    private Vector3 _smoothedCollisionPosition;
    private readonly RaycastHit[] _collisionHits = new RaycastHit[8];

    public void ResetCollisionSmoothing(Vector3 currentPosition)
    {
        _smoothedCollisionPosition = currentPosition;
        _hasSmoothedCollisionPosition = false;
    }

    public Vector3 ResolveCollisionAdjustedPosition(Vector3 desiredPosition, Vector3 pivotPosition, Vector3 up, Transform player)
    {
        if (_collisionSphereRadius <= 0f || _collisionPositionSharpness <= 0f)
            return desiredPosition;

        Vector3 rayOrigin = pivotPosition + (up * _collisionRayHeight);
        Vector3 toCamera = desiredPosition - rayOrigin;
        float desiredDistance = toCamera.magnitude;

        Vector3 resolvedPosition = desiredPosition;
        if (desiredDistance > 0.0001f)
        {
            Vector3 direction = toCamera / desiredDistance;

            if (TryGetCameraObstruction(rayOrigin, direction, desiredDistance, player, out RaycastHit hit))
            {
                float safeDistance = Mathf.Clamp(hit.distance - _collisionWallPadding, _collisionMinDistance, desiredDistance);
                float blockedAmount = 1f - Mathf.Clamp01(safeDistance / desiredDistance);
                float verticalDrop = _collisionMaxDrop * blockedAmount;
                resolvedPosition = rayOrigin + (direction * safeDistance) - (up * verticalDrop);
            }
        }

        if (!_hasSmoothedCollisionPosition)
        {
            _smoothedCollisionPosition = resolvedPosition;
            _hasSmoothedCollisionPosition = true;
            return _smoothedCollisionPosition;
        }

        float deltaTime = GetSafeDeltaTime();
        float t = 1f - Mathf.Exp(-_collisionPositionSharpness * deltaTime);
        _smoothedCollisionPosition = Vector3.Lerp(_smoothedCollisionPosition, resolvedPosition, t);
        return _smoothedCollisionPosition;
    }

    private bool TryGetCameraObstruction(Vector3 origin, Vector3 direction, float distance, Transform player, out RaycastHit closestHit)
    {
        closestHit = default;
        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            _collisionSphereRadius,
            direction,
            _collisionHits,
            distance,
            _cameraCollisionMask,
            QueryTriggerInteraction.Ignore);

        bool foundHit = false;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = _collisionHits[i];

            if (hit.collider == null)
                continue;

            if (player != null && hit.collider.transform.IsChildOf(player))
                continue;

            if (hit.distance < bestDistance)
            {
                bestDistance = hit.distance;
                closestHit = hit;
                foundHit = true;
            }
        }

        return foundHit;
    }

    private float GetSafeDeltaTime()
    {
        if (Application.isPlaying)
            return Mathf.Max(Time.deltaTime, 0.0001f);

        return 1f / 60f;
    }
}
