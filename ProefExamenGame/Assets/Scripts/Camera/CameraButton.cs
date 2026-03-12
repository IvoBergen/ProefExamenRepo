using UnityEngine;

public class CameraButton : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private POVSwitcher _povSwitcher;

    public void changeCamera()
    {
        if (_povSwitcher == null)
            return;

        _povSwitcher.ToggleMode();
    }
}
