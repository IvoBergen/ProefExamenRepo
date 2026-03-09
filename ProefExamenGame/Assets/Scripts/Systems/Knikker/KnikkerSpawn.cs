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


    private void SpawnBall()
    {
        if (_ballPrefab == null || _spawnPoint == null || _waypoints == null || _waypoints.Length == 0)
        {
            return;
        }

        GameObject newBall = Instantiate(_ballPrefab, _spawnPoint.position, _spawnPoint.rotation);

        Knikker ball = newBall.GetComponent<Knikker>();

        if (ball == null)
        {
            return;
        }

        ball.SetPath(_waypoints);
    }
}
