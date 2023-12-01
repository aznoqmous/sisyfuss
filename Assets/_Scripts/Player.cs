using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;
    void Awake()
    {
        Instance= this;

        _material = _spriteRenderers[0].material;
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.color = Color.white;
            sr.material = _material;
        }
        _material.SetColor("_Color", _color);
    }

    bool _isMouseControl;

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Transform _handTransform;
    [SerializeField] Transform _arrow;
    [SerializeField] CollisionCheck _groundCheck;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _spriteContainer;
    [SerializeField] List<SpriteRenderer> _spriteRenderers;
    [SerializeField] Color _color;
    Material _material;
    float _lastDamageTime = -999f;
    float _damageDuration = 0.2f;

    public Rigidbody2D RigidBody {get { return _rigidBody;  } }

    [Header("Movement")]
    [SerializeField] float _speed = 2f;
    [SerializeField] float _carryingSpeed = 0.8f;
    [SerializeField] float _jumpForce = 2f;
    [SerializeField] float _gravity = 1f;
    [SerializeField] float _fallGravityFactor = 2f;
    [SerializeField] int _maxJumps = 2;
    int _jumpCount = 0;

    [SerializeField] float _attractionDistance = 10f;
    public float AttractionDistance { get { return _attractionDistance; } }

    [Header("Ball")]
    [SerializeField] float _maxThrowingTime = 1f;
    [SerializeField] float _throwForce = 1f;
    [SerializeField] float _ropingForce = 1f;
    float _linkLength = 10f;
    float _throwingTimeScale = 1f;
    float _jumpShotForce = 1f;

    public Ball Ball { get { return _pickedUpBall; } }
    public float LinkLength { get { return _linkLength; } }

    [Header("Health")]
    [SerializeField] int _maxHealth = 3;
    [SerializeField] GameObject _heartBreakPrefab;
    int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } }
    public int MaxHealth { get { return _maxHealth; } }
    public bool IsDead { get { return _currentHealth <= 0; } }

    public UpgradeList UpgradeList = new UpgradeList();

    float _startThrowing = 0f;
    float _throwingRatio { get { return Mathf.Min(1, (Time.unscaledTime - _startThrowing) / _maxThrowingTime); } }
    bool _isGrounded = false;
    Vector2 _lastInput;

    [Header("Audio")]
    [SerializeField] List<AudioClip> _audioThrows;
    [SerializeField] AudioClip _audioJump;
    [SerializeField] AudioClip _audioLand;
    [SerializeField] AudioClip _audioBlow;
    [SerializeField] AudioClip _audioNewSkill;

    int _enemyLayerMask;
    private void Start()
    {
        _currentHealth = _maxHealth;

        GameManager.Instance.UpdateHealthContainers();
        GameManager.Instance.UpdatePlayerHealth();
        GameManager.Instance.UpdatePlayerCoins();

        GameManager.Instance.SetTimeScale(1f);

        if (_pickedUpBall != null) _pickedUpBall.Attach(this);
        _enemyLayerMask = LayerMask.GetMask("Walkable");

    }

    void Update()
    {
        /* Inputs */
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            _isMouseControl = false;
            MousePointer.Instance.SetState(_isMouseControl);
        }
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseControl = true;
            MousePointer.Instance.SetState(_isMouseControl);
        }
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Q)) _movement.x -= 1;

        /* Damaged */
        _material.SetFloat("_Ratio", Time.time - _lastDamageTime < _damageDuration ? 0 : 1);
        if (Time.time - _lastDamageTime < _damageDuration) GameManager.Instance.SetTimeScale(1f, Time.unscaledDeltaTime);
        else GameManager.Instance.SetTimeScale(1f);

        if (IsDead) GameManager.Instance.SetTimeScale(0.1f, Time.unscaledDeltaTime);

        _isGrounded = _groundCheck.IsColliding || _selfGroundCheck;

        /* Animation */
        if (_movement.sqrMagnitude > 0.01f) _lastInput = _movement; 
        _animator.SetFloat("SpeedX", Mathf.Abs(_rigidBody.velocity.x));
        _animator.SetFloat("SpeedY", _rigidBody.velocity.y);
        _animator.SetBool("IsGrounded", _isGrounded);
        _spriteContainer.localScale = new Vector3(Mathf.Sign(_lastInput.x), 1, 1);

        HandleJump();

        HandlePull();

        HandleThrow();

        if (_isCarrying)
        {
            _pickedUpBall.transform.position = _handTransform.position;

        }

        HandleMovement();
    }

    Vector2 _movement = new Vector2();
    void HandleMovement()
    {
        float speed = _speed;
        Vector2 movement = _movement;
        movement.x *= speed;
        //if (_isCarrying) movement.x /= 2f;
        movement.y = _rigidBody.velocity.y;

        if(!_throwing) _rigidBody.velocity = movement;

        Vector3 position = transform.position;
        position.y += 2f;
        Vector3 target = Vector3.Lerp(Camera.main.transform.position, position, Time.unscaledDeltaTime * 2f);
        target.z = -15f;
        Camera.main.transform.position = target;

        _rigidBody.gravityScale = (_isGrounded || movement.y > 0 ? 1f : _fallGravityFactor) * _gravity;
    }

    bool _aired = false;
    void HandleJump()
    {
        if (_isGrounded && _rigidBody.velocity.y < 0) _jumpCount = 0;
        if (_maxJumps > _jumpCount && InputHelper.GetJumpDown())
        {
            var velocity = _rigidBody.velocity;
            velocity.y = 0;
            _rigidBody.velocity = velocity;
            _rigidBody.AddForce(Vector2.up * _jumpForce * 100f);
            _jumpCount++;
            if(_isGrounded) _aired = false;
            AudioManager.Instance.PlaySound(_audioJump);
        }
        
        if(!_aired && !_isGrounded)
        {
            _aired = true;
        }
        if(_aired && _isGrounded)
        {
            _aired = false;
            _jumpCount = 0;
            AudioManager.Instance.PlaySound(_audioLand);
        }
    }
    void HandlePull()
    {
        if (!_isCarrying && _pickedUpBall != null && InputHelper.GetActionButton())
        {
            Vector2 force = (transform.position - _pickedUpBall.transform.position).normalized * Time.deltaTime * 1000f;
            _pickedUpBall.Throw(force * _pickedUpBall.Mass * _ropingForce);
            //_animator.SetBool("IsRoping", true);

            _spriteContainer.localScale = new Vector3(Mathf.Sign((_pickedUpBall.transform.position.x - transform.position.x)), 1, 1);

        }
        else
        {
            //_animator.SetBool("IsRoping", false);
        }
    }
    void HandleThrow()
    {
        // Start throw
        if (_isCarrying && InputHelper.GetActionButtonDown())
        {
            _throwing = true;
            _startThrowing = Time.unscaledTime;
            _arrow.gameObject.SetActive(true);
        }

        // Throw
        if (_isCarrying && _throwing && InputHelper.GetActionButtonUp())
        {
            //_animator.SetBool("IsHolding", false);
            //_animator.SetTrigger("IsThrowing");
            //_rigidBody.mass -= _pickedUpMass;
            if (_isMouseControl) _movement = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            _pickedUpBall.EnablePhysics(true);
            Vector3 force = _movement * 100f * _throwForce * _throwingRatio * _pickedUpBall.Mass;
            if (_aired) force *= _jumpShotForce;
            _pickedUpBall.Throw(force);
            _isCarrying = false;
            _throwing = false;
            GameManager.Instance.SetTimeScale(1f);
            _arrow.gameObject.SetActive(false);

            AudioManager.Instance.PlaySound(_audioThrows.PickRandom());
        }


        if (_throwing)
        {
            if (_isMouseControl) _movement = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

            _handTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg);
            _handTransform.localScale = Vector3.one * (0.2f + 0.8f * _throwingRatio);
            GameManager.Instance.SetTimeScale(_throwingTimeScale);

        }
    }

    [Header("Ball")]
    [SerializeField] Ball _pickedUpBall;
    bool _isCarrying = false;
    public bool IsCarrying { get { return _isCarrying; } }
    bool _throwing = false;

    public void SelectBall(Ball ball)
    {
        if (_pickedUpBall != null) _pickedUpBall.Detach();
        _pickedUpBall = ball;
        _pickedUpBall.Attach(this);
        _isCarrying = false;
    }
    void PickUp(Ball ball)
    {
        if (_pickedUpBall)
        {
            _pickedUpBall.EnablePhysics();
        }

        _pickedUpBall = ball;
        _isCarrying = true;
        ball.EnablePhysics(false);
        _handTransform.position = transform.position + new Vector3(0, ball.transform.localScale.x*0.6f, 0);
    }

    bool _selfGroundCheck = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Foe foe = collision.collider.GetComponent<Foe>();
        if(foe != null)
        {
            TakeDamage(1);
            _rigidBody.velocity = (Vector2)(transform.position - foe.transform.position).normalized * 20f;
        }
        if (collision.collider.GetComponent<TerrainManager>())
        {
            _selfGroundCheck = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Ball ball = collision.collider.GetComponent<Ball>();
        if (ball != null)
        {
            if(InputHelper.GetActionButton()) PickUp(ball);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<TerrainManager>())
        {
            _selfGroundCheck = false;
        }
    }

    public void TakeDamage(float amount=1)
    {
        if (Time.time - _lastDamageTime < _damageDuration) return;
        GameManager.Instance.SetTimeScale(0f);
        _lastDamageTime = Time.time;
        _currentHealth -= Mathf.FloorToInt(amount);
        AudioManager.Instance.PlaySound(_audioBlow);
        if (_currentHealth <= 0) Die();
       
        
        GameManager.Instance.UpdatePlayerHealth();
        Instantiate(_heartBreakPrefab, transform.position + Vector3.up, Quaternion.identity);
    }

    public void Heal(float amount=1)
    {
        _currentHealth += Mathf.FloorToInt(amount);
        GameManager.Instance.UpdatePlayerHealth();
    }

    public void Die()
    {
        LevelManager.Instance.LoadScene("LevelScene");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    float _coins = 0f;
    public int Coins { get { return Mathf.FloorToInt(_coins); } }

    public void GainCoin(float amount = 1)
    {
        _coins += amount;
        GameManager.Instance.UpdatePlayerCoins();
    }
    public void LoseCoin(float amount = 1)
    {
        _coins -= amount;
        GameManager.Instance.UpdatePlayerCoins();
    }

    public void AddUpgrade(ScriptableUpgrade su) 
    {
        AudioManager.Instance.PlaySound(_audioNewSkill);
        UpgradeList.AddUpgrade(su);
        AudioManager.Instance.PlayMainTheme();
        switch (su.Type)
        {
            case UpgradeType.AirThunder:
                break;
            case UpgradeType.RollFire:
                break;
            case UpgradeType.BallSize:
                _pickedUpBall.Size *= 1.2f;
                break;
            case UpgradeType.MaxJumps:
                _maxJumps++;
                break;
            case UpgradeType.MoveSpeed:
                _speed *= 1.2f;
                break;
            case UpgradeType.Roping:
                _ropingForce *= 1.2f;
                break;
            case UpgradeType.Throwing:
                _throwForce *= 1.2f;
                break;

            case UpgradeType.MaxHealth:
                _maxHealth++;
                GameManager.Instance.UpdateHealthContainers();
                Heal(1);
                break;
            case UpgradeType.LinkLength:
                _linkLength *= 1.2f;
                _pickedUpBall.SetLinkLength(_linkLength);
                break;
            case UpgradeType.FasterChargeTime:
                _maxThrowingTime /= 1.2f;
                break;
            case UpgradeType.JumpShot:
                _jumpShotForce *= 1.2f;
                GameManager.Instance.Unlocks.Unlock(UnlockType.BouncingBall);
                break;
            case UpgradeType.LinkDamage:
                _linkDamage *= 1.2f;
                _pickedUpBall.EnableLinkDamage();
                break;
            case UpgradeType.StoneLaser:
                break;
            case UpgradeType.MegaThrow:
                break;
            default:
                break;
        }
    }

    float _linkDamage = 3f;
    public float LinkDamage { get { return _linkDamage; } }
}
