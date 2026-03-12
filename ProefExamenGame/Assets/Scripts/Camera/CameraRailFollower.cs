using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// <c>CameraRailFollower</c> Follows a spline rail while staying focused on the player.
/// </summary>
[ExecuteAlways]
public class CameraRailFollower : MonoBehaviour
{
    public enum CameraMode
    {
        POV,
        SplineOrbit
    }

    [Header("References")]
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Transform _player;

    [Header("Camera Offset")]
    [SerializeField] private float _sideOffset;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _backOffset;
    [SerializeField] private float _pitchOffset;

    [Header("Jump Follow")]
    [Range(0f, 1f)]
    [SerializeField] private float _verticalFollowWeight;

    [Header("Spline Search")]
    [Range(0.001f, 0.5f)]
    private float _localSearchWindow = 0.08f;

    [Range(3, 64)]
    private int _localSearchSamples = 15;

    [Range(8, 256)]
    private int _globalSearchSamples = 64;

    private bool _allowBackwardMovement = true;

    [Header("Follow")]
    private Vector3 _worldUp = Vector3.up;

    [Header("Rotation Bias")]
    [Range(0f, 1f)]
    [SerializeField] private float _playerRotationWeight = 0.65f;

    [Header("Spline End")]
    private float _splineEndThreshold = 0.001f;

    [Header("Mode")]
    [SerializeField] private CameraMode _cameraMode = CameraMode.POV;
    [SerializeField] private float _modeTransitionSharpness = 12f;
    [SerializeField] private int _orbitClosestGlobalSamples = 256;
    [SerializeField] private int _orbitClosestRefineIterations = 4;
    [SerializeField] private float _orbitClosestRefineWindow = 0.03f;

    [Header("Spline Orbit Offset")]
    [SerializeField] private float _splineOrbitSideOffset;
    [SerializeField] private float _splineOrbitHeightOffset;
    [SerializeField] private float _splineOrbitBackOffset;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask _cameraCollisionMask = ~0;
    [SerializeField] private float _collisionSphereRadius = 0.25f;
    [SerializeField] private float _collisionWallPadding = 0.15f;
    [SerializeField] private float _collisionMinDistance = 1f;
    [SerializeField] private float _collisionRayHeight = 1.2f;
    [SerializeField] private float _collisionMaxDrop = 2f;
    [SerializeField] private float _collisionPositionSharpness = 14f;

    private float _currentT = 0f;
    private bool _hasAcquiredTarget;

    private float _currentVerticalFollow;

    private Vector3 _currentPositionDirection;
    private Vector3 _currentRotationDirection;
    private Vector3 _initialPlayerForward;
    private Vector3 _stablePlayerForward;
    private float _cachedSideOffset;
    private bool _isFrozenAtSplineEnd;
    private Vector3 _frozenEndCameraPosition;
    private Quaternion _frozenEndCameraRotation;
    private bool _hasSmoothedCollisionPosition;
    private Vector3 _smoothedCollisionPosition;
    private CameraMode _lastCameraMode;
    private bool _isModeTransitioning;
    private Vector3 _modeTransitionStartPosition;
    private Quaternion _modeTransitionStartRotation;
    private float _modeTransitionProgress = 1f;
    private readonly RaycastHit[] _collisionHits = new RaycastHit[8];

    private void OnEnable()
    {
        CacheInitialPlayerForward();
        _currentT = FindBestTOnWholeSpline();
        _hasAcquiredTarget = true;
        _isFrozenAtSplineEnd = false;
        _frozenEndCameraPosition = transform.position;
        _frozenEndCameraRotation = transform.rotation;
        _hasSmoothedCollisionPosition = false;
        _smoothedCollisionPosition = transform.position;
        _lastCameraMode = _cameraMode;
        _isModeTransitioning = false;
        _modeTransitionProgress = 1f;
        _modeTransitionStartPosition = transform.position;
        _modeTransitionStartRotation = transform.rotation;

        CacheSideOffset(_currentT);
        UpdateCameraOnSpline(_currentT);
    }

    private void LateUpdate()
    {
        if (!HasValidSpline())
            return;

        if (!_hasAcquiredTarget)
        {
            _currentT = FindBestTOnWholeSpline();
            _hasAcquiredTarget = true;
        }

        float targetT = _cameraMode == CameraMode.SplineOrbit
            ? FindClosestTToPlayerOnSpline()
            : FindBestNearbyT(_currentT);

        if (_cameraMode == CameraMode.POV && !_allowBackwardMovement && targetT < _currentT)
            targetT = _currentT;

        _currentT = targetT;

        _currentT = Mathf.Clamp01(_currentT);
        UpdateCameraOnSpline(_currentT);
    }

    /// <summary>
    /// Finds the closest point on the spline using a full search.
    /// </summary>
    private float FindBestTOnWholeSpline()
    {
        if (!HasValidSpline() || _player == null)
            return _currentT;

        int sampleCount = Mathf.Max(8, _globalSearchSamples);

        float bestT = 0f;
        float bestScore = float.MaxValue;

        for (int i = 0; i < sampleCount; i++)
        {
            float sampleT = (sampleCount == 1) ? 0f : (float)i / (sampleCount - 1);
            float score = DistanceFromPlayerToSplinePoint(sampleT);

            if (score < bestScore)
            {
                bestScore = score;
                bestT = sampleT;
            }
        }

        return bestT;
    }

    /// <summary>
    /// Finds the best nearby spline position around the current progress.
    /// </summary>
    private float FindBestNearbyT(float centerT)
    {
        if (!HasValidSpline() || _player == null)
            return centerT;

        int sampleCount = Mathf.Max(3, _localSearchSamples);

        float minT = Mathf.Clamp01(centerT - _localSearchWindow);
        float maxT = Mathf.Clamp01(centerT + _localSearchWindow);

        if (!_allowBackwardMovement)
            minT = centerT;

        float bestT = centerT;
        float bestScore = float.MaxValue;

        for (int i = 0; i < sampleCount; i++)
        {
            float normalized = (sampleCount == 1) ? 0f : (float)i / (sampleCount - 1);
            float sampleT = Mathf.Lerp(minT, maxT, normalized);
            float score = DistanceFromPlayerToSplinePoint(sampleT);

            if (score < bestScore)
            {
                bestScore = score;
                bestT = sampleT;
            }
        }

        return bestT;
    }

    private float DistanceFromPlayerToSplinePoint(float t)
    {
        Vector3 splinePosition = _splineContainer.EvaluatePosition(Mathf.Clamp01(t));
        return (_player.position - splinePosition).sqrMagnitude;
    }

    /// <summary>
    /// Finds the continuous spline coordinate nearest to the player's world position.
    /// </summary>
    private float FindClosestTToPlayerOnSpline()
    {
        if (!HasValidSpline() || _player == null)
            return _currentT;

        int coarseSamples = Mathf.Max(16, _orbitClosestGlobalSamples);
        float bestT = 0f;
        float bestScore = float.MaxValue;

        for (int i = 0; i < coarseSamples; i++)
        {
            float sampleT = (coarseSamples == 1) ? 0f : (float)i / (coarseSamples - 1);
            float score = DistanceFromPlayerToSplinePoint(sampleT);
            if (score < bestScore)
            {
                bestScore = score;
                bestT = sampleT;
            }
        }

        int refineIterations = Mathf.Max(0, _orbitClosestRefineIterations);
        float refineWindow = Mathf.Max(0.0005f, _orbitClosestRefineWindow);

        for (int iteration = 0; iteration < refineIterations; iteration++)
        {
            float minT = Mathf.Clamp01(bestT - refineWindow);
            float maxT = Mathf.Clamp01(bestT + refineWindow);
            const int refineSamples = 9;

            for (int i = 0; i < refineSamples; i++)
            {
                float normalized = (refineSamples == 1) ? 0f : (float)i / (refineSamples - 1);
                float sampleT = Mathf.Lerp(minT, maxT, normalized);
                float score = DistanceFromPlayerToSplinePoint(sampleT);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestT = sampleT;
                }
            }

            refineWindow *= 0.5f;
        }

        return Mathf.Clamp01(bestT);
    }

    /// <summary>
    /// Applies the camera position and rotation for the current spline progress.
    /// </summary>
    private void UpdateCameraOnSpline(float t)
    {
        if (!HasValidSpline())
            return;

        t = Mathf.Clamp01(t);

        Vector3 splinePosition = _splineContainer.EvaluatePosition(t);
        Vector3 splineTangent = _splineContainer.EvaluateTangent(t);

        Vector3 up = _worldUp.normalized;
        Vector3 splineForward = Vector3.ProjectOnPlane(splineTangent, up).normalized;

        if (splineForward.sqrMagnitude < 0.0001f)
            return;

        Vector3 playerForward = GetStablePlayerForward(up, splineForward);
        Vector3 targetOffsetDirection = GetBlendedDirection(splineForward, playerForward, _playerRotationWeight);
        _currentPositionDirection = targetOffsetDirection;
        _currentRotationDirection = targetOffsetDirection;

        Vector3 offsetForward = _currentPositionDirection;
        Vector3 rotationForward = _currentRotationDirection;

        Vector3 offsetRight = Vector3.Cross(up, offsetForward).normalized;

        float targetVerticalFollow = 0f;
        if (_player != null)
        {
            float playerVerticalDelta = _player.position.y - splinePosition.y;
            targetVerticalFollow = playerVerticalDelta * _verticalFollowWeight;
        }

        _currentVerticalFollow = targetVerticalFollow;

        Vector3 pivotPosition = GetCameraPivotPosition(splinePosition);
        Vector3 cameraPosition = GetModeCameraPosition(
            _cameraMode,
            splinePosition,
            pivotPosition,
            offsetRight,
            offsetForward,
            up);
        Quaternion targetRotation = Quaternion.LookRotation(rotationForward, up) * Quaternion.Euler(_pitchOffset, 0f, 0f);

        if (IsAtSplineEnd(t))
        {
            if (!_isFrozenAtSplineEnd)
            {
                _frozenEndCameraPosition = cameraPosition;
                _frozenEndCameraRotation = targetRotation;
                _isFrozenAtSplineEnd = true;
            }

            cameraPosition = _frozenEndCameraPosition;
            targetRotation = _frozenEndCameraRotation;
        }
        else
        {
            _isFrozenAtSplineEnd = false;
        }

        cameraPosition = ResolveCollisionAdjustedPosition(cameraPosition, pivotPosition, up);
        HandleModeTransitionChange();
        ApplyModeTransition(ref cameraPosition, ref targetRotation);

        if (_cameraMode == CameraMode.SplineOrbit && _player != null)
        {
            Vector3 lookDirection = _player.position - cameraPosition;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                targetRotation = Quaternion.LookRotation(lookDirection.normalized, up);
            }
        }

        transform.position = cameraPosition;
        transform.rotation = targetRotation;
    }

    public void SetCameraMode(CameraMode mode)
    {
        _cameraMode = mode;
    }

    public CameraMode CurrentMode => _cameraMode;

    private bool HasValidSpline()
    {
        return _splineContainer != null && _splineContainer.Spline != null;
    }

    /// <summary>
    /// Uses the player's planar rotation as the camera heading source.
    /// </summary>
    private Vector3 GetStablePlayerForward(Vector3 up, Vector3 splineForward)
    {
        if (_player == null)
            return _initialPlayerForward.sqrMagnitude > 0.0001f ? _initialPlayerForward : splineForward;

        Vector3 planarForward = Vector3.ProjectOnPlane(_player.forward, up);
        if (planarForward.sqrMagnitude > 0.0001f)
        {
            _stablePlayerForward = planarForward.normalized;
        }

        if (_stablePlayerForward.sqrMagnitude > 0.0001f)
        {
            return _stablePlayerForward;
        }

        if (_initialPlayerForward.sqrMagnitude > 0.0001f)
        {
            return _initialPlayerForward;
        }

        return splineForward;
    }

    private Vector3 GetBlendedDirection(Vector3 splineDirection, Vector3 movementDirection, float movementWeight)
    {
        Vector3 blendedDirection = Vector3.Lerp(splineDirection, movementDirection, movementWeight);
        return blendedDirection.sqrMagnitude > 0.0001f ? blendedDirection.normalized : splineDirection;
    }

    private void CacheInitialPlayerForward()
    {
        Vector3 up = _worldUp.normalized;
        _initialPlayerForward = Vector3.forward;
        _stablePlayerForward = Vector3.zero;

        if (_player == null)
            return;

        Vector3 planarForward = Vector3.ProjectOnPlane(_player.forward, up);
        if (planarForward.sqrMagnitude > 0.0001f)
        {
            _initialPlayerForward = planarForward.normalized;
            _stablePlayerForward = _initialPlayerForward;
        }
    }

    private void CacheSideOffset(float t)
    {
        _cachedSideOffset = 0f;

        if (!HasValidSpline() || _player == null)
            return;

        Vector3 up = _worldUp.normalized;
        Vector3 splinePosition = _splineContainer.EvaluatePosition(Mathf.Clamp01(t));
        Vector3 splineTangent = _splineContainer.EvaluateTangent(Mathf.Clamp01(t));
        Vector3 splineForward = Vector3.ProjectOnPlane(splineTangent, up).normalized;

        if (splineForward.sqrMagnitude <= 0.0001f)
            return;

        Vector3 playerForward = GetStablePlayerForward(up, splineForward);
        Vector3 offsetForward = GetBlendedDirection(splineForward, playerForward, _playerRotationWeight);

        if (offsetForward.sqrMagnitude <= 0.0001f)
            return;

        Vector3 offsetRight = Vector3.Cross(up, offsetForward).normalized;
        _cachedSideOffset = Vector3.Dot(splinePosition - _player.position, offsetRight);
    }

    private bool IsAtSplineEnd(float t)
    {
        return t >= 1f - _splineEndThreshold;
    }

    private Vector3 GetModeCameraPosition(
        CameraMode mode,
        Vector3 splinePosition,
        Vector3 pivotPosition,
        Vector3 offsetRight,
        Vector3 offsetForward,
        Vector3 up)
    {
        if (mode == CameraMode.SplineOrbit)
        {
            Vector3 splineOrbitOffset =
                (offsetRight * _splineOrbitSideOffset) +
                (up * (_splineOrbitHeightOffset + _currentVerticalFollow)) -
                (offsetForward * _splineOrbitBackOffset);

            return splinePosition + splineOrbitOffset;
        }

        Vector3 orbitOffset =
            (offsetRight * (_sideOffset + _cachedSideOffset)) +
            (up * (_heightOffset + _currentVerticalFollow)) -
            (offsetForward * _backOffset);

        return pivotPosition + orbitOffset;
    }

    private Vector3 GetCameraPivotPosition(Vector3 fallbackPosition)
    {
        if (_player != null)
        {
            return _player.position;
        }

        return fallbackPosition;
    }

    private void HandleModeTransitionChange()
    {
        if (_cameraMode == _lastCameraMode)
            return;

        _lastCameraMode = _cameraMode;
        _isModeTransitioning = true;
        _modeTransitionProgress = 0f;
        _modeTransitionStartPosition = transform.position;
        _modeTransitionStartRotation = transform.rotation;
    }

    private void ApplyModeTransition(ref Vector3 targetPosition, ref Quaternion targetRotation)
    {
        if (!_isModeTransitioning)
            return;

        if (_modeTransitionSharpness <= 0f)
        {
            _isModeTransitioning = false;
            _modeTransitionProgress = 1f;
            return;
        }

        float deltaTime = GetSafeDeltaTime();
        float step = 1f - Mathf.Exp(-_modeTransitionSharpness * deltaTime);
        _modeTransitionProgress = Mathf.Clamp01(_modeTransitionProgress + step);

        targetPosition = Vector3.Lerp(_modeTransitionStartPosition, targetPosition, _modeTransitionProgress);
        targetRotation = Quaternion.Slerp(_modeTransitionStartRotation, targetRotation, _modeTransitionProgress);

        if (_modeTransitionProgress >= 0.999f)
        {
            _isModeTransitioning = false;
            _modeTransitionProgress = 1f;
        }
    }

    private Vector3 ResolveCollisionAdjustedPosition(Vector3 desiredPosition, Vector3 pivotPosition, Vector3 up)
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

            if (TryGetCameraObstruction(rayOrigin, direction, desiredDistance, out RaycastHit hit))
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

    private bool TryGetCameraObstruction(Vector3 origin, Vector3 direction, float distance, out RaycastHit closestHit)
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

            if (_player != null && hit.collider.transform.IsChildOf(_player))
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
