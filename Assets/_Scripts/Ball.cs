using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] CircleCollider2D _collider;
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] DistanceJoint2D _joint;
    [SerializeField] ParticleSystem _groundParticles;
    [SerializeField] Transform _irisTransform;
    [SerializeField] UnlockType _unlockType;

    public bool IsUnlocked { get { return GameManager.Instance.Unlocks.IsUnlocked(_unlockType); } }

    bool _isGrounded = false;

    public float ChainWidth
    {
        get { return _lineRenderer.startWidth; }
        set { _lineRenderer.startWidth = value; _lineRenderer.endWidth = value; }
    }
    public Vector2 Velocity { get { return _rigidBody.velocity;  } }
    public float Mass { get { return _rigidBody.mass; } }
    public float Size
    {
        get { return transform.localScale.x; }
        set { transform.localScale = Vector3.one * value; }
    }
    [SerializeField] float _bounciness = 0f;

    [SerializeField] List<AudioClip> _dropClips;

    bool _isAttached = false;
    public bool IsAttached { get { return _isAttached; } }

    private void Start()
    {
        var emission = _groundParticles.emission;
        emission.enabled = true;
    }

    public void Update()
    {
        Vector3 direction = (Player.Instance.transform.position - transform.position);
        _irisTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        if (!_isAttached) return;
        if (_joint.connectedBody == null) return;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _joint.connectedBody.transform.position);

        var main = _groundParticles.main;
        main.startColor = TerrainManager.Instance.GetDustColorAtPosition(transform.position);

        if (_isAttached)
        {
            _lineRenderer.transform.position = (Player.Instance.transform.position - transform.position) / 2 + transform.position;
        }

        HandleAirThunder();
        HandleRollFire();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _lastGroundedTime = Time.time;

        if(collision.collider.GetComponent<Player>() == null)
        {
            Vector3 velocity = _rigidBody.velocity;
            velocity.y = collision.relativeVelocity.y * _bounciness;
            _rigidBody.velocity = velocity;
        }
        
        if (collision.collider.GetComponent<TerrainManager>() != null)
        {
            var emission = _groundParticles.emission;
            emission.enabled = true;
            _isGrounded = true;

            if(collision.relativeVelocity.magnitude > 1f) AudioManager.Instance.PlaySound(_dropClips.PickRandom());
            //_rigidBody.AddForce(collision.relativeVelocity * 10f);
        }

        Foe foe = collision.collider.GetComponent<Foe>();
        if (foe != null && Velocity.sqrMagnitude > 1f)
        {
            float damages = Mathf.Sqrt(collision.relativeVelocity.magnitude) * Size;
            foe.TakeDamage(damages);
            if (!foe.IsDead) SetVelocity(-Velocity);
            HandleAirThunderHit(foe, damages);
            HandleRollFireHit(foe, damages);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<TerrainManager>() != null)
        {
            _groundParticles.transform.position = collision.contacts[0].point;
            _lastGroundedTime = Time.time;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<TerrainManager>() != null)
        {
            var emission = _groundParticles.emission;
            emission.enabled = false;
            _lastGroundedTime = Time.time;
            _isGrounded = false;
        }
    }

    public void EnablePhysics(bool state = true)
    {
        if (!state)
        {
            _rigidBody.velocity = Vector2.zero;
            var emission = _groundParticles.emission;
            emission.enabled = false;
        }
        _collider.isTrigger = !state;
        _rigidBody.isKinematic = !state;
    }

    public void Throw(Vector2 force)
    {
        _rigidBody.AddForce(force);
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rigidBody.velocity = velocity;
    }

    [Header("AirThunder")]
    [SerializeField] ParticleSystem _electricityParticleSystem;
    [SerializeField] Electricity _electricityPrefab;
    [SerializeField] List<AudioClip> _electricityLoadClips;
    float _lastGroundedTime;
    float _airThunderTime = 1f;
    bool _airThunderActive = false;
    void HandleAirThunder()
    {

        if (Player.Instance.UpgradeList.AirThunder <= 0f) return;
        if (Player.Instance.IsCarrying) _lastGroundedTime = Time.time;
        if(!_electricityParticleSystem.gameObject.activeInHierarchy) _electricityParticleSystem.gameObject.SetActive(true);

        var emission = _electricityParticleSystem.emission;
        emission.enabled = _airThunderActive;

        if (Time.time - _lastGroundedTime > _airThunderTime)
        {
            if(!_airThunderActive) AudioManager.Instance.PlaySound(_electricityLoadClips.PickRandom());
            _airThunderActive = true;
        }
        else
        {
            _airThunderActive = false;
        }
    }
    void HandleAirThunderHit(Foe foe, float damages)
    {
        if (!_airThunderActive) return;
        _airThunderActive = false;
        Electricity e = Instantiate(_electricityPrefab, transform.position, Quaternion.identity);
        e.FindTarget();
        e.SetDamages(damages);
    }

    [Header("RollFire")]
    [SerializeField] ParticleSystem _fireParticleSystem;
    [SerializeField] Explosion _explosionPrefab;
    bool _rollFireIsActive = false;
    float _charged = 0f;
    void HandleRollFire()
    {
        if (Player.Instance.UpgradeList.RollFire <= 0f) return;
        if (!_fireParticleSystem.gameObject.activeInHierarchy) _fireParticleSystem.gameObject.SetActive(true);
        var emission = _fireParticleSystem.emission;
        emission.enabled = _rollFireIsActive;
        var gndEmission = _groundParticles.emission;
        if(gndEmission.enabled && _rollFireIsActive) gndEmission.enabled = false;
        if (!_rollFireIsActive)
        {
            if (Velocity.magnitude < 1f) _charged = 0;
            if (Velocity.magnitude > 8f && _isGrounded)
            {
                _charged += Time.deltaTime * Velocity.magnitude / 10f;
                if (_charged >= 1f) _rollFireIsActive = true;
            }
        }
    }
    void HandleRollFireHit(Foe foe, float damages)
    {
        if (!_rollFireIsActive) return;
        _rollFireIsActive = false;
        _charged = 0f;
        Explosion explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        explosion.SetDamages(damages);
        explosion.Size = 2f;
    }

    [Header("Link Damage")]
    [SerializeField] LinkDamage _linkDamage;
    public void EnableLinkDamage(bool state = true)
    {
        _linkDamage.gameObject.SetActive(state);
    }

    public void Detach()
    {
        EnablePhysics(false);
        _joint.connectedBody = null;
        _isAttached = false;
        _lineRenderer.enabled = false;
        EnableLinkDamage(false);
    }
    public void Attach(Player player)
    {
        transform.parent = player.transform.parent;
        _isAttached = true;
        _joint.connectedBody = player.RigidBody;
        _lineRenderer.enabled = true;
        SetLinkLength(player.LinkLength);
        EnablePhysics();
        if(Player.Instance.UpgradeList.LinkDamage >= 0) EnableLinkDamage(false);
    }

    public void SetLinkLength(float length)
    {
        _joint.distance= length;
    }
}
