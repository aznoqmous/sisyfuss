using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foliage : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] float _apparitionDistance = 40f;
    [SerializeField] float _minSize = 1f;
    [SerializeField] float _maxSize = 1f;
    [SerializeField] List<AudioClip> _audios = new List<AudioClip>();
    [SerializeField] AudioSource _audioSource;
    [SerializeField] SpriteRenderer _spriteRenderer;

    float _size;
    private void Start()
    {
        _size = Random.Range(_minSize, _maxSize);
    }

    private void Update()
    {
        if (Mathf.Abs(Player.Instance.transform.position.x - transform.position.x) > 100f) Destroy(gameObject);
        transform.localScale = Vector3.Lerp(
            transform.localScale, 
            Mathf.Abs(Player.Instance.transform.position.x - transform.position.x) < _apparitionDistance ? Vector3.one * _size : Vector3.zero, 
            Time.deltaTime * 5f
        );
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetTrigger("Contact");
        _audioSource.volume = AudioManager.Instance.SoundVolume;
        _audioSource.clip = _audios.PickRandom();
        _audioSource.Play();
    }
    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}
