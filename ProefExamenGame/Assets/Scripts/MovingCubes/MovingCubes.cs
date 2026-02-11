using UnityEngine;

public class MovingCubes : MonoBehaviour
{
    [SerializeField] private GameObject _obstacle;
    [SerializeField] private float _speed = 1f;

    private int _direction;
    private float _moveTime = 4f;

    private void Start()
    {
        MoveInRandomDirection();
    }
    private void RandomDirection()
    {
        _direction = Random.Range(1, 4);
    }

    private void MoveInRandomDirection()
    {
        RandomDirection();
        switch (_direction)
        {
            default:
                RandomDirection();
                break;
            case 1:
                _obstacle.transform.position = Vector3.forward * _speed * Time.deltaTime;
                new WaitForSeconds(_moveTime);
                RandomDirection();
                _direction = 0;
                break;
            case 2:
                _obstacle.transform.position = Vector3.forward * _speed * Time.deltaTime;
                new WaitForSeconds(_moveTime);
                RandomDirection();
                _direction = 0;
                break;
            case 3:
                _obstacle.transform.position = Vector3.forward * _speed * Time.deltaTime;
                new WaitForSeconds(_moveTime);
                RandomDirection();
                _direction = 0;
                break;
            case 4:
                _obstacle.transform.position = Vector3.forward * _speed * Time.deltaTime;
                new WaitForSeconds(_moveTime);
                RandomDirection();
                _direction = 0;
                break;
        }

    }
}
