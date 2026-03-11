using UnityEngine;


public class KnikkerSpawn : MonoBehaviour
{
    #region References

    [Header("References")]

    [SerializeField] private GameObject[] _ballPrefabs;
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
    /// Selects a random ball prefab and spawns it at the spawn point.
    /// The spawned ball receives the waypoint path to follow.
    /// </summary>
    private void SpawnBall()
    {
        if (_ballPrefabs == null || _ballPrefabs.Length == 0)
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

        int randomIndex = Random.Range(0, _ballPrefabs.Length);

        GameObject selectedPrefab = _ballPrefabs[randomIndex];

        GameObject spawnedBall = Instantiate(
            selectedPrefab,
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