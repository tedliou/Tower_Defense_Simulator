using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TowerController : MonoBehaviour
{
    [Header("Settings")]
    public float sensorDistnace = 4;
    public float cooldown = 1;
    public float shootSpeed = 1;

    [Header("Components")]
    public Transform gun;

    [Header("Data")]
    public EnemyController[] enemies;

    private float _cooldown;

    private void Awake()
    {
        enemies = new EnemyController[] { };
        _cooldown = 0;
    }

    private void Start()
    {
        FindInRangeEnemies();
    }

    private void Update()
    {
        _cooldown += Time.deltaTime;
        if (_cooldown >= cooldown)
        {
            _cooldown = 0;
            FindInRangeEnemies();
            Fire();
        }
    }

    private void FindInRangeEnemies()
    {
        var colliders = Physics.OverlapSphere(transform.position, sensorDistnace, 1 << 6);
        enemies = new EnemyController[colliders.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            var enemy = colliders[i].GetComponent<EnemyController>();
            enemies[i] = enemy;

            if (i > 0)
            {
                var distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                for (int j = 0; j < i; j++)
                {
                    var compareDistance = Vector3.Distance(transform.position, enemies[j].transform.position);
                    if (distance < compareDistance)
                    {
                        var tmp = enemies[j];
                        enemies[j] = enemies[i];
                        enemies[i] = tmp;
                    }
                }
            }
        }

        
    }

    private void Fire()
    {

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        sensorDistnace = Mathf.Max(0, sensorDistnace);
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, sensorDistnace);

        if (enemies.Length > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gun.transform.position, enemies[0].transform.position);
        }
    }
#endif
}
