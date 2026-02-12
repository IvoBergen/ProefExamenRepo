using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    public float force = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Shock")
        {
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection =- pushDirection.normalized;

            GetComponent<Rigidbody>().AddForce(pushDirection * force * 100);
        }
    }
}
