using UnityEngine;

/// <summary>
/// Handles time-based level events such as countdown notifications and triggering the game over UI.
/// Typically called by a timer or animation event system.
/// </summary>
public class LevelTriggerer : MonoBehaviour
{
    /// <summary>Reference to the Game Over UI object that becomes visible when the timer reaches zero.</summary>
    [SerializeField] GameObject _uiObject;

    void Start()
    {
    }

    /// <summary>
    /// Called when 90 seconds remain.
    /// Can be used to trigger audio, UI, or gameplay changes.
    /// </summary>
    public void negetigseconde()
    {
        Debug.Log("90");
    }

    /// <summary>
    /// Called when 60 seconds remain.
    /// </summary>
    public void zestigseconde()
    {
        Debug.Log("60");
    }

    /// <summary>
    /// Called when 30 seconds remain.
    /// </summary>
    public void dertigeconde()
    {
        Debug.Log("30");
    }

    /// <summary>
    /// Called when the timer reaches zero.
    /// Activates the Game Over UI.
    /// </summary>
    public void gameOver()
    {
        _uiObject.SetActive(true);
        Debug.Log("0");
    }
}
