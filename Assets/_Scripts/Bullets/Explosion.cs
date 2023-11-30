using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    [SerializeField] Collider2D _collider;

    public float Size
    {
        get
        {
            return transform.localScale.x;
        }
        set
        {
            transform.localScale = Vector3.one * value;
        }
    }

    float _damages = 1f;
    public void SetDamages(float damages)
    {
        _damages = damages;
    }

    private void Start()
    {
        Destroy(gameObject, 1.5f);        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Foe foe = collision.GetComponent<Foe>();
        if(foe != null)
        {
            Physics2D.IgnoreCollision(collision, _collider);
            foe.TakeDamage(_damages);
        }
    }
}
