using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    public float healthPoint = 2;
    public Vector3 hpBarOffset;

    [Header("Components")]
    public Slider hpBar;

    private void Start()
    {
        hpBar.value = hpBar.maxValue = healthPoint;
        var canvas = FindObjectOfType<Canvas>().transform;
        var position = Camera.main.WorldToScreenPoint(transform.position);
        hpBar = Instantiate(hpBar, canvas);
        hpBar.transform.position = position + hpBarOffset;
    }

    private void OnDestroy()
    {
        Destroy(hpBar.gameObject);
    }

    public void SetDamage(float damage)
    {
        UpdateHealth(healthPoint - damage);
    }

    private void UpdateHealth(float health)
    {
        healthPoint = health;
        if (healthPoint <= 0)
        {
            // Die
            Destroy(gameObject);
        }
        hpBar.value = healthPoint;
    }
}
