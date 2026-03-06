using System.Collections;
using UnityEngine;
/// <summary>
/// <c>PowerUpSpeedBoost</c> is a power up that increases the movement speed of the player for a set duration.
/// </summary>
public class PowerUpSpeedBoost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    private float _increasedMovementSpeed = 5f;

    private float _durationTime = 2f;


    private void OnEnable()
    {
        PowerUpPickUp.onPowerUpPickUp += StartSpeedBoost;
    }

    /// <summary>
    /// <c>StartSpeedBoost</c> gets called when a power up is picked up and start the coroutine for the increased movement.
    /// </summary>
    private void StartSpeedBoost()
    {
        StartCoroutine(SpeedBoost());
    }

    private IEnumerator SpeedBoost()
    {
        _playerController._moveSpeed += _increasedMovementSpeed;
        Debug.Log("started");
        yield return new WaitForSecondsRealtime(_durationTime);

        _playerController._moveSpeed = _playerController._originalMovementSpeed;
        Debug.Log("ended");
        yield break;
    }


}
