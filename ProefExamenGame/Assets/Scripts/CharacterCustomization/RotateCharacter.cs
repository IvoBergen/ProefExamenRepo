using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private float _rotationSpeed;

    private float _currentYRotation = 0f;

    private void Update()
    {
        _currentYRotation += _rotationSpeed * Time.deltaTime;
        _player.transform.rotation = Quaternion.Euler(0f, _currentYRotation, 0f);
    }
}
