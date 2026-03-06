using System;
using UnityEngine;

public class InkSpot : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private string _target;

    public static event Action onInkEntered;
    public static event Action onInkExited;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_target))
        {
            onInkEntered?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_target))
        {
            onInkExited?.Invoke();
        }

    }
}
