using System.Collections;
using UnityEngine;

/// <summary>
/// <c>CubeRotation</c> Rotates the cube to a random value on the Z or X axis andueue up a new random rotation when it reaches the target rotation.
/// </summary>
public class CubeRotation : MonoBehaviour
{
    [Header("Rotation Values")]
    [SerializeField] private float[] _rotationValues;

    private Quaternion _targetRotation;
    private int _randomDir;

    [Header("Rotation Settings")]
    public float rotationSpeed;
    public float rotationDelay;

    private void Start()
    {
        _targetRotation = RotateX();
        StartCoroutine(RotateTowardsTarget());
    }

    private IEnumerator RotateTowardsTarget()
    {
        while (Quaternion.Angle(transform.rotation, _targetRotation) > 0.001f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        var randomRotDir = Random.Range(0, 2);

        _targetRotation = randomRotDir == 0 ? RotateX() : RotateZ();

        yield return new WaitForSeconds(rotationDelay);

        yield return StartCoroutine(RotateTowardsTarget());
    }

    /// <summary>
    /// <c>RotateX</c> Grabs a random value from an array and rotates the cube on the X-axis to that value, while keeping the Y and Z rotation the same.
    /// </summary>
    /// <returns>The value for the cube to move to</returns>
    private Quaternion RotateX()
    {
        _randomDir = Random.Range(0, _rotationValues.Length);

        _targetRotation = Quaternion.Euler(_rotationValues[_randomDir], transform.rotation.eulerAngles.y, _rotationValues[0]);

        return _targetRotation;

    }

    /// <summary>
    /// <c>RotateZ</c> Grabs a random value from an array and rotates the cube on the Z-axis to that value, while keeping the X and Y rotation the same.
    /// </summary>
    /// <returns>The value for the cube to move to</returns>
    private Quaternion RotateZ()
    {
        _randomDir = Random.Range(0, _rotationValues.Length);

        _targetRotation = Quaternion.Euler(_rotationValues[0], transform.rotation.eulerAngles.y, _rotationValues[_randomDir]);

        return _targetRotation;

    }
}
