using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayCollider : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] Collider2D _walkingCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
       Physics2D.IgnoreCollision(collision, _walkingCollider, collision.gameObject.transform.position.y + collision.offset.y < transform.position.y);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Physics2D.IgnoreCollision(collision, _walkingCollider, collision.gameObject.transform.position.y + collision.offset.y < transform.position.y);
    }
}
