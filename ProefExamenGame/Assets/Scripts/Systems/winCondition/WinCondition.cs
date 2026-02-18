using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Used to trigger the win condition 
/// </summary>
public class WinCondition : MonoBehaviour
{

    [SerializeField] UnityEvent Eventwin;
    [SerializeField] GameObject StatsUi;
    private void OnTriggerEnter(Collider other)
    {
        win();
    }
    /// <summary>
    /// activates the win event.
    /// </summary>
    void win()
    {
        Eventwin.Invoke();
        StatsUi.SetActive(true);
    }
}