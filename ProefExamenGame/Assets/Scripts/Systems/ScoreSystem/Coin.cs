using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    /// <summary>
    /// Acts as a coin, rotates and fires an event when collected
    /// </summary>

    [SerializeField] private UnityEvent collectedCoin;
    [SerializeField] private float rotateSpeed = 120f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        collectedCoin?.Invoke();
        Debug.Log("collected");
        Destroy(gameObject);
    }
}
