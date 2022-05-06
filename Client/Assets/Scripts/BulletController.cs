using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage;

    private void Awake()
    {
        Destroy(gameObject, 1);
    }

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
            other.GetComponent<EnemyController>().SetDamage(damage);
            Destroy(gameObject);
        }
    }
}
