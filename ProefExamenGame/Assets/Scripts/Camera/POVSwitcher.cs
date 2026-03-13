using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// <c>POVSwitcher</c> owns camera mode selection and switching.
/// </summary>
public class POVSwitcher : MonoBehaviour
{
    public enum CameraMode
    {
        POV,
        SplineOrbit
    }

    [Header("POV Switcher Settings")]
    [SerializeField] private CameraRailFollower _cameraRailFollower;
    [SerializeField] private CameraMode _startMode = CameraMode.POV;
    [SerializeField] private float _modeTransitionSharpness = 12f;

    [Header("Spline Orbit Search")]
    [SerializeField] private int _orbitClosestGlobalSamples = 256;
    [SerializeField] private int _orbitClosestRefineIterations = 4;
    [SerializeField] private float _orbitClosestRefineWindow = 0.03f;

    [Header("Spline Orbit Offset")]
    [SerializeField] private float _splineOrbitSideOffset;
    [SerializeField] private float _splineOrbitHeightOffset;
    [SerializeField] private float _splineOrbitBackOffset;

    [Header("Optional Keyboard Toggle")]
    [SerializeField] private bool _allowKeyboardToggle;
    [SerializeField] private KeyCode _toggleKey = KeyCode.V;

    public CameraMode CurrentMode { get; private set; }

    private CameraMode _lastTransitionMode;
    private bool _isModeTransitioning;
    private Vector3 _modeTransitionStartPosition;
    private Quaternion _modeTransitionStartRotation;
    private float _modeTransitionProgress = 1f;

    private void Awake()
    {
        SetMode(_startMode);
        _lastTransitionMode = CurrentMode;
        _isModeTransitioning = false;
        _modeTransitionProgress = 1f;
    }

    private void Update()
    {
        if (_allowKeyboardToggle && Input.GetKeyDown(_toggleKey))
        {
            ToggleMode();
        }
    }

    public void ToggleMode()
    {
        CameraMode nextMode = CurrentMode == CameraMode.POV
            ? CameraMode.SplineOrbit
            : CameraMode.POV;

        SetMode(nextMode);
    }

    public void SetMode(CameraMode mode)
    {
        CurrentMode = mode;

        if (_cameraRailFollower != null)
        {
            _cameraRailFollower.SetCameraMode(mode);
        }
    }

    public void ApplyModeTransition(
        Vector3 currentCameraPosition,
        Quaternion currentCameraRotation,
        ref Vector3 targetPosition,
        ref Quaternion targetRotation)
    {
        HandleModeTransitionChange(currentCameraPosition, currentCameraRotation);

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

    public float ResolveTargetSplineT(SplineContainer splineContainer, Transform player, float fallbackT, float nearbyT)
    {
        if (CurrentMode != CameraMode.SplineOrbit)
            return nearbyT;

        return FindClosestTToPlayerOnSpline(splineContainer, player, fallbackT);
    }

    public Vector3 ResolveModeCameraPosition(
        Vector3 splinePosition,
        Vector3 pivotPosition,
        Vector3 offsetRight,
        Vector3 offsetForward,
        Vector3 up,
        float currentVerticalFollow,
        float povSideOffset,
        float povHeightOffset,
        float povBackOffset)
    {
        if (CurrentMode == CameraMode.SplineOrbit)
        {
            Vector3 splineOrbitOffset =
                (offsetRight * _splineOrbitSideOffset) +
                (up * (_splineOrbitHeightOffset + currentVerticalFollow)) -
                (offsetForward * _splineOrbitBackOffset);

            return splinePosition + splineOrbitOffset;
        }

        Vector3 povOffset =
            (offsetRight * povSideOffset) +
            (up * (povHeightOffset + currentVerticalFollow)) -
            (offsetForward * povBackOffset);

        return pivotPosition + povOffset;
    }

    public Quaternion ResolveModeRotation(Quaternion baseRotation, Vector3 cameraPosition, Transform player, Vector3 up)
    {
        if (CurrentMode != CameraMode.SplineOrbit || player == null)
            return baseRotation;

        Vector3 lookDirection = player.position - cameraPosition;
        if (lookDirection.sqrMagnitude <= 0.0001f)
            return baseRotation;

        return Quaternion.LookRotation(lookDirection.normalized, up);
    }

    private float FindClosestTToPlayerOnSpline(SplineContainer splineContainer, Transform player, float fallbackT)
    {
        if (splineContainer == null || splineContainer.Spline == null || player == null)
            return fallbackT;

        int coarseSamples = Mathf.Max(16, _orbitClosestGlobalSamples);
        float bestT = 0f;
        float bestScore = float.MaxValue;

        for (int i = 0; i < coarseSamples; i++)
        {
            float sampleT = (coarseSamples == 1) ? 0f : (float)i / (coarseSamples - 1);
            float score = DistanceFromPlayerToSplinePoint(splineContainer, player, sampleT);
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
                float score = DistanceFromPlayerToSplinePoint(splineContainer, player, sampleT);

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

    private float DistanceFromPlayerToSplinePoint(SplineContainer splineContainer, Transform player, float t)
    {
        Vector3 splinePosition = splineContainer.EvaluatePosition(Mathf.Clamp01(t));
        return (player.position - splinePosition).sqrMagnitude;
    }

    private void HandleModeTransitionChange(Vector3 currentCameraPosition, Quaternion currentCameraRotation)
    {
        if (CurrentMode == _lastTransitionMode)
            return;

        _lastTransitionMode = CurrentMode;
        _isModeTransitioning = true;
        _modeTransitionProgress = 0f;
        _modeTransitionStartPosition = currentCameraPosition;
        _modeTransitionStartRotation = currentCameraRotation;
    }

    private float GetSafeDeltaTime()
    {
        if (Application.isPlaying)
            return Mathf.Max(Time.deltaTime, 0.0001f);

        return 1f / 60f;
    }
}
