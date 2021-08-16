using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float minHealth = 1.0f;
    public float maxHealth = 10.0f;

    float health = 10.0f;

    private void Start()
    {
        health = Random.Range(minHealth, maxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            health -= 1;
            if (health <= 0) Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}