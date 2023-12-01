using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Laser : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] List<AudioClip> _hitClips;
    [SerializeField] ParticleSystem _particleSystem;

    public void AddForce(Vector2 force)
    {
        _rigidBody.AddForce(force * _rigidBody.mass);
    }

    float _damages = 1f;

    void Start()
    {
        _damages = 4f + Player.Instance.UpgradeList.StoneLaser;
    }

    private void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(_rigidBody.velocity.y, _rigidBody.velocity.x) * Mathf.Rad2Deg);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TerrainManager ground = collision.GetComponent<TerrainManager>();
        if (ground != null)
        {
            AudioManager.Instance.PlaySound(_hitClips.PickRandom());
            Die();
        }

        Foe foe = collision.GetComponent<Foe>();
        if (foe != null)
        {
            foe.TakeDamage(_damages);
            _collider.enabled = false;
            Die();
        }
    }

    void Die()
    {
        AudioManager.Instance.PlaySound(_hitClips.PickRandom());
        _particleSystem.Play();
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        Destroy(gameObject, 1f);
    }
}
