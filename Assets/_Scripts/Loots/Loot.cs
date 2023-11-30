using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] ParticleSystem _lootParticles;
    [SerializeField] Collider2D _collider;
    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] AudioClip _audioOnLoot;
    [SerializeField] AudioSource _audioSource;

    float _birth = 0f;
    public bool _looted = false;
    private void Start()
    {
        if (_birth == 0f) _birth = Time.time;
        _collider.enabled = false;
        _looted = false;
    }
    private void Update()
    {
        if (_looted) return;
        if(Time.time - _birth > 0.5f)
        {
            _collider.enabled = true;

            if (Player.Instance.transform.position.DistanceTo(transform.position) < Player.Instance.AttractionDistance)
            {
                //_rigidBody.MovePosition(transform.position + (Player.Instance.transform.position - transform.position).normalized * Time.deltaTime * 50f);
                _rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, (Player.Instance.transform.position - transform.position).normalized * Time.deltaTime * 2000f, Time.deltaTime * 10f);
            }

        }
    }


    void PlayerLoot(Player player)
    {
        OnLoot(player);
        _looted = true;
        _lootParticles.gameObject.SetActive(true);
        _lootParticles.Stop();
        _lootParticles.Play();
        _collider.enabled = false;
        _spriteRenderer.enabled = false;

        _audioSource.volume = AudioManager.Instance.SoundVolume * 2f;
        _audioSource.clip = _audioOnLoot;
        _audioSource.pitch = Random.Range(0.9f, 2f);
        _audioSource.Play();


        Destroy(gameObject, 1f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Player>() != null || collision.collider.GetComponent<Ball>() != null) PlayerLoot(Player.Instance);
    }

    protected virtual void OnLoot(Player player)
    {

    }

    public void AddForce(Vector2 direction)
    {
        _rigidBody.AddForce(direction);
    }
}
