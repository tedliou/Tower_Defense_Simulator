using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public void Fire(Vector3 direction, Vector3 startPosition)
    {
        transform.position = startPosition;
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
