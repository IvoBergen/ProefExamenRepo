using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Respawn(other.gameObject);
        }
    }

    private void Respawn(GameObject player)
    {
        if (CheckPointManager.Instance == null || CheckPointManager.Instance.CurrentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint set!");
            return;
        }

        Transform respawnLocation = CheckPointManager.Instance.CurrentCheckpoint.transform;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // temporarily disable physics
        }

        player.transform.position = respawnLocation.position;
        player.transform.rotation = respawnLocation.rotation;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false; // re-enable physics
        }
    }
}
