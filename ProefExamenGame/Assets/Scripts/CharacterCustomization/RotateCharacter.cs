using UnityEngine;

/// <summary>
/// <c>RotateCharacter</c> Rotates the character slowly for a preview in the main menu
/// </summary>

public class RotateCharacter : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private float _currentYRotation = 0f;

    private void Update()
    {
        _currentYRotation += _rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, _currentYRotation, 0f);
    }
}