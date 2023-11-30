using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PhantomBoss : Boss
{
    
    [SerializeField] FlyMovement _movement;
    [SerializeField] Animator _animator;
    [SerializeField] ParticleSystem _speakParticleSystem;

    [SerializeField] float _cooldown = 5f;
    float _lastCastTime = 0f;

    protected override void OnActivate()
    {
        _lastCastTime = Time.time;
    }

    protected override void Cast()
    {
        if (Time.time - _lastCastTime < _cooldown) return;
        
        _lastCastTime = Time.time;

        if (_attacksLeft.Count <= 0) Think();

        switch (_currentAttack)
        {
            case PhantomAttack.None:
                Speak();
                break;
            case PhantomAttack.StoneFall:
                CastStoneFall();
                break;
            case PhantomAttack.Lava:
                CastLava();
                break;
            default:
                break;
        }

        _attacksLeft.Remove(_currentAttack);
    }

    List<PhantomAttack> _attacks = new List<PhantomAttack>() { 
        PhantomAttack.None,
        PhantomAttack.StoneFall,
        PhantomAttack.Lava
    };
    List<PhantomAttack> _attacksLeft = new List<PhantomAttack>() {
        PhantomAttack.StoneFall,
        PhantomAttack.None,
    };
    PhantomAttack _currentAttack { get { return _attacksLeft[0]; } }

    void Think()
    {
        _attacksLeft = new List<PhantomAttack>(_attacks);
        _attacksLeft.Shuffle();
    }



    [Header("Stone Fall")]
    [SerializeField] AudioClip _stoneFallClip;
    [SerializeField] PhantomStone _stonePrefab;
    int _stoneCount = 6;
    float _stoneInterval = 0.5f;
    List<PhantomStone> _stones = new List<PhantomStone>();
    void CastStoneFall()
    {
        _animator.SetTrigger("CastStoneFall");
        AudioManager.Instance.PlaySound(_stoneFallClip);
    }

    public void StoneFall() {
        float directionX = Mathf.Sign(Player.Instance.transform.position.x - transform.position.x);
        for(float i = 0; i < _stoneCount; i++)
        {
            Vector3 position = Player.Instance.transform.position;
            position.y += 15f;
            position.x += directionX * (i * 5f - _stoneCount / 2 * 5f + Random.value);
            StartCoroutine(SpawnStone(position, i * _stoneInterval));
        }
    }

    IEnumerator SpawnStone(Vector3 position, float timeout=0f)
    {
        yield return new WaitForSeconds(timeout);
        _stones.Add(Instantiate(_stonePrefab, position, Quaternion.identity));
    }

    [Header("Lava")]
    [SerializeField] AudioClip _lavaClip;
    [SerializeField] GameObject _lavaPrefab;
    int _lavaCount = 3;
    float _lavaInterval = 0.7f;

    void CastLava()
    {
        _animator.SetTrigger("CastLava");
        AudioManager.Instance.PlaySound(_lavaClip);
    }

    void Lava()
    {
        for (float i = 0; i < _lavaCount; i++)
        {
            StartCoroutine(SpawnLava(transform.position, i * _lavaInterval));
        }
    }

    [Header("Rain")]
    [SerializeField] GameObject _bulletPrefab;


    IEnumerator SpawnLava(Vector3 position, float timeout = 0f)
    {
        yield return new WaitForSeconds(timeout);
        GameObject lava = Instantiate(_lavaPrefab, position, Quaternion.identity);
        Vector3 scale = lava.transform.localScale;
        scale.x *= Mathf.Sign(Player.Instance.transform.position.x - transform.position.x);
        lava.transform.localScale = scale; 
    }


    public void Speak()
    {
        _speakParticleSystem.Stop();
        _speakParticleSystem.Play();
    }

    protected override void OnDeath()
    {
        GameManager.Instance.Unlocks.Unlock(UnlockType.FloatingBall);
    }

    void Erase()
    {
        Destroy(gameObject, 1f);
    }
}

public enum PhantomAttack
{
    None,
    StoneFall,
    Lava
}