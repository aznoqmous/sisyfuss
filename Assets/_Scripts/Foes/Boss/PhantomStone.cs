using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomStone : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] Collider2D _surfaceCollider;
    [SerializeField] ContactDamage _contactDamage;
    [SerializeField] ParticleSystem _dustParticleSystem;
    [SerializeField] List<AudioClip> _rumbleClips;
    [SerializeField] AudioSource _audioSource;
    private void Update()
    {
        if(!_rigidBody.isKinematic && _rigidBody.velocity.magnitude <= 0.1f)
        {
            _surfaceCollider.gameObject.SetActive(true);
            _contactDamage.gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        _surfaceCollider.gameObject.SetActive(false);
    }

    public void Fall()
    {
        _rigidBody.isKinematic = false;
        _rigidBody.velocity = Vector2.down;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.GetComponent<TerrainManager>() != null)
        {
            Dust();
        }
    }

    public void Dust()
    {
        _audioSource.clip = _rumbleClips.PickRandom();
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.Play();
        Instantiate(_dustParticleSystem, transform.position, Quaternion.identity);
    }

    public void Erase()
    {
        _collider.enabled = false;
        _surfaceCollider.enabled = false;
        Destroy(gameObject, 2f);
    }
}
