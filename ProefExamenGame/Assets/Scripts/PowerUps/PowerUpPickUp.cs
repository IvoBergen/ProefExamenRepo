using System;
using UnityEngine;
/// <summary>
/// <cPowerUpPickUp</c> is a base class for picking up different power-ups through the use of an Action.
/// </summary>
public class PowerUpPickUp : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private string _target;

    public static event Action onPowerUpPickUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_target))
        {
            onPowerUpPickUp?.Invoke();
            Destroy(gameObject);
        }
    }
}
