using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] List<AudioClip> _hitClips;

    public void AddForce(Vector2 force)
    {
        _rigidBody.AddForce(force * _rigidBody.mass);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            player.TakeDamage();
            Die();
            return;
        }

        TerrainManager ground = collision.GetComponent<TerrainManager>();
        if(ground != null)
        {
            AudioManager.Instance.PlaySound(_hitClips.PickRandom());
            Die();
        }

        Ball ball = collision.GetComponent<Ball>();
        if (ball != null)
        {
            AudioManager.Instance.PlaySound(_hitClips.PickRandom());
            Die();
            return;
        }
    }

    void Die()
    {
        _particleSystem.Play();
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        Destroy(gameObject, 1f);
    }
}
