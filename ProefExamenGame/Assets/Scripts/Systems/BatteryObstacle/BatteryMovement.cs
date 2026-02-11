using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryMovement : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up; 

    public Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
    }

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(
            _rotationAxis * _rotationSpeed * Time.fixedDeltaTime
        );

        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }

}
