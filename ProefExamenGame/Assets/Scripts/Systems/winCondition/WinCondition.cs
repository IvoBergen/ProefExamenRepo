using UnityEngine;
using UnityEngine.Events;

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