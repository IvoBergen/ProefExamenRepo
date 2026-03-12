using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraButton : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CameraRailFollower _crf;
    // Start is called before the first frame update
    public void changeCamera()
    {
        if  (_crf.CurrentMode == CameraRailFollower.CameraMode.POV)
            {
                _crf.SetCameraMode(CameraRailFollower.CameraMode.SplineOrbit);
            }
        else
            {
                _crf.SetCameraMode(CameraRailFollower.CameraMode.POV);
            }
    }
}
