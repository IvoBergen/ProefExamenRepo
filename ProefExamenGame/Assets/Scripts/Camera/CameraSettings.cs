using UnityEngine;

/// <summary>
/// <c>CameraSettings</c> Controls camera settings and relative movement calculations.
/// </summary>

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    public Vector3 GetCameraRelativeMovement(Vector2 input)
    {
        if (_cameraTransform == null)
        {
            return Vector3.zero;
        }

        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraRight * input.x) + (cameraForward * input.y);

        return moveDirection;
    }
}
