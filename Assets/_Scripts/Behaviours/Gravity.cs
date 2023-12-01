using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] ParticleSystem _particleSystem;

    public float Power = 1f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        if(rigidbody != null)
            rigidbody.AddForce((transform.position - collision.gameObject.transform.position) * Time.time / 5f * Power);
    }
    public void SetSize(float size=1f)
    {
        transform.localScale = Vector3.one * size;
    }
    public void SetState(bool state)
    {
        _collider.enabled = state;
        var emission = _particleSystem.emission;
        emission.enabled = state;
    }
}
