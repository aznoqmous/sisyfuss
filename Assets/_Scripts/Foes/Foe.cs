using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Foe : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] float _health = 1;
    float _currentHealth = 1;
    public bool IsDead { get { return _currentHealth <= 0;  } }
    [SerializeField] ParticleSystem _damageParticleSystem;

    [SerializeField] GameObject _spriteObject;
    [SerializeField] List<SpriteRenderer> _spriteRenderers;
    [SerializeField] Color _color;
    Material _material;
    float _lastDamageTime = -999f;
    float _damageDuration = 0.2f;

    [SerializeField] List<FoePod> _pods = new List<FoePod>();
    public List<FoePod> Pods { get { return _pods; } }

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] FixedJoint2D _joint;
    [SerializeField] Animator _animator;

    public Rigidbody2D RigidBody { get { return _rigidBody; } }
    public FixedJoint2D Joint { get { return _joint; } }

    bool _canMove = true;
    public bool CanMove { get { return _canMove; } }

    [SerializeField] Movement _movement;
    public Movement Movement { get { return _movement; } }

    [SerializeField] List<Foe> _spawnedFoes = new List<Foe>();
    [SerializeField] float _coinsDrop;

    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _audioHurt;

    private void Awake()
    {
        foreach (FoePod pod in _pods) pod.SetFoe(this);

        _material = _spriteRenderers[0].material;
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            //sr.color = Color.white;
            sr.material = _material;
        }
        //_material.SetColor("_Color", _color);
    }
    private void Start()
    {
        _health *= GameManager.Instance.Difficulty;
        _currentHealth = _health;

        EntityManager.Instance.Add(this);
    }

    private void Update()
    {
        _material.SetFloat("_Ratio", Time.time - _lastDamageTime < _damageDuration ? 0 : 1);
    }

    public void TakeDamage(float amount)
    {
        amount = Mathf.FloorToInt(amount);
        StatusManager.Instance.AddStatus(amount.ToString(), transform.position + Vector3.up);
        _lastDamageTime = Time.time;
        _currentHealth -= amount;
        _audioSource.volume = AudioManager.Instance.SoundVolume;
        _audioSource.clip = _audioHurt;
        _audioSource.Play();
        if (_currentHealth <= 0) Die();

        DetachPods();
    }


    [Header("Death")]
    [SerializeField] string _deathAnimationTrigger = "";
    [SerializeField] ParticleSystem _deathParticleSystem;
    [SerializeField] AudioClip _deathClip;
    void Die()
    {
        _collider.enabled = false;
        if(_deathParticleSystem != null )
        {
            _deathParticleSystem.Stop();
            _deathParticleSystem.Play();
        }
        else if (_damageParticleSystem != null) _damageParticleSystem.Play();

        if (_deathAnimationTrigger == "") Erase();
        else _animator.SetTrigger(_deathAnimationTrigger);

        if (_deathClip != null) AudioManager.Instance.PlaySound(_deathClip);

        DetachPods();
        foreach (Foe foe in _spawnedFoes) EntityManager.Instance.SpawnFoe(foe, transform.position);
        if (LootManager.Instance.PlayerMissingHealthRandom(0f, 0.05f)) LootManager.Instance.SpawnHeart(transform.position);
        else LootManager.Instance.SpawnCoins(transform.position, _coinsDrop, 30);
    }

    void Erase()
    {
        _spriteObject.SetActive(false);
        EntityManager.Instance.Remove(this);
        Destroy(gameObject, 1f);
    }

    void DetachPods()
    {
        foreach (FoePod pod in _pods)
        {
            pod.Detach();
            pod.Disable();
        }
    }

    float _savedMass;
    public void PreventMovement()
    {
        _savedMass = _rigidBody.mass;
        _rigidBody.mass = 0f;
        _canMove= false;
        _animator.SetBool("IsAttached", true);
    }
    public void EnableMovement()
    {
        _rigidBody.mass = _savedMass;
        _canMove= true;
        _animator.SetBool("IsAttached", false);
    }

    public List<FoePod> GetAvailablePods()
    {
        return _pods.Where((FoePod pod) => pod.IsAvailable).ToList();
    }

    public FoePod[] GetCompatiblePod(Foe foe)
    {
        foreach(FoePod pod in GetAvailablePods())
        {
            FoePod foePod = pod.CanConnectToFoe(foe);
            if (foePod != null) return new FoePod[] { pod, foePod };
        }
        return null;
    }

    public Foe ConnectTo(Foe foe)
    {
        FoePod[] podList = GetCompatiblePod(foe);
        if(podList == null) return foe;
        podList[0].AttachTo(podList[1]);
        return foe;
    }

}
