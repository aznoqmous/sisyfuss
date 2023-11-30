using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] float _health = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TakeDamage(collision.relativeVelocity.magnitude);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        TakeDamage(1);
    }

    void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0f) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
