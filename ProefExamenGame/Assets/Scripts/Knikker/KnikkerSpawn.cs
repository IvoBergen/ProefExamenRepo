using UnityEngine;

public class KnikkerSpawn : MonoBehaviour
{
    #region References

    [Header("References")]

    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform[] _waypoints;

    #endregion


    #region Spawn Settings

    [Header("Spawn Settings")]

    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private bool _spawnOnStart = true;

    #endregion


    private float _spawnTimer;


    private void Start()
    {
        if (_spawnOnStart)
        {
            SpawnBall();
        }
    }


    private void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer < _spawnInterval)
        {
            return;
        }

        _spawnTimer = 0f;
        SpawnBall();
    }


    /// <summary>
    /// Spawns a new ball at the spawn point and gives it the configured waypoint path.
    /// The method stops safely if required references are missing.
    /// </summary>
    private void SpawnBall()
    {
        if (_ballPrefab == null)
        {
            return;
        }

        if (_spawnPoint == null)
        {
            return;
        }

        if (_waypoints == null || _waypoints.Length == 0)
        {
            return;
        }

        GameObject spawnedBall = Instantiate(
            _ballPrefab,
            _spawnPoint.position,
            _spawnPoint.rotation
        );

        Knikker ballComponent = spawnedBall.GetComponent<Knikker>();

        if (ballComponent == null)
        {
            return;
        }

        ballComponent.SetPath(_waypoints);
    }
}