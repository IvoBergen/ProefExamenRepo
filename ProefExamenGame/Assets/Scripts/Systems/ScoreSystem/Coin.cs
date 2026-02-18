using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Acts as a coin, rotates and fires an event when collected
/// </summary>
public class Coin : MonoBehaviour
{


    [SerializeField] private UnityEvent _collectedCoin;
    [SerializeField] private float _rotateSpeed = 120f;

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
