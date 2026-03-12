using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// <c>CameraRailFollower</c> Follows a spline rail while staying focused on the player.
/// </summary>
[ExecuteAlways]
public class CameraRailFollower : MonoBehaviour
{
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

    private void OnEnable()
    {
        CacheInitialPlayerForward();
        _currentT = FindBestTOnWholeSpline();
        _hasAcquiredTarget = true;
        _isFrozenAtSplineEnd = false;
        _frozenEndCameraPosition = transform.position;
        _frozenEndCameraRotation = transform.rotation;

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

        float targetT = FindBestNearbyT(_currentT);

        if (!_allowBackwardMovement && targetT < _currentT)
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

        Vector3 orbitOffset =
            (offsetRight * (_sideOffset + _cachedSideOffset)) +
            (up * (_heightOffset + _currentVerticalFollow)) -
            (offsetForward * _backOffset);

        Vector3 cameraPosition = GetOrbitCameraPosition(orbitOffset, splinePosition);
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

        transform.position = cameraPosition;
        transform.rotation = targetRotation;
    }

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

    private Vector3 GetOrbitCameraPosition(Vector3 orbitOffset, Vector3 fallbackPosition)
    {
        if (_player != null)
        {
            return _player.position + orbitOffset;
        }

        return fallbackPosition + orbitOffset;
    }
}
