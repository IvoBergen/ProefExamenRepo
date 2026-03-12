using UnityEngine;

/// <summary>
/// <c>CameraButton</c> is a simple script that allows the player to switch between different camera perspectives by calling the ToggleMode method on the POVSwitcher component when the button is pressed.
/// </summary>
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
