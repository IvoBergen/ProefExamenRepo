using UnityEngine;

public class TargetFramerate : MonoBehaviour
{
    [SerializeField, Tooltip("Target framerate for the application")] private int targetFramerate = 300;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFramerate;
    }
}
