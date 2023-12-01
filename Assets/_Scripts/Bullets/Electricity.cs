using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    [SerializeField] int _rebounds = 2;
    [SerializeField] float _speed = 50f;
    [SerializeField] float _distance = 10f;
    [SerializeField] Transform _target;
    [SerializeField] List<AudioClip> _hitClips;
    [SerializeField] Collider2D _collider;
    float _damages = 1f;

    private void Start()
    {
        _distance = 10f + 2f * Player.Instance.UpgradeList.AirThunder;
        _rebounds = Player.Instance.UpgradeList.AirThunder;
    }

    public void SetDamages(float damages)
    {
        _damages = damages;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetRebounds(int rebounds)
    {
        _rebounds = rebounds;
    }

    

    private void Update()
    {
        if(_target != null)
        {
            transform.position += (_target.position - transform.position).normalized * _speed * Time.deltaTime;
        }
        else
        {
            _collider.enabled = false;
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Foe foe = collision.GetComponent<Foe>();
        if(foe != null)
        {
            if (_rebounds <= 0f) return;
            AudioManager.Instance.PlaySound(_hitClips.PickRandom());
            foe.TakeDamage(_damages);
            _rebounds--;
            if (_rebounds <= 0f)
            {
                _target = null;
                _collider.enabled = false;
                Destroy(gameObject, 1f);
                return;
            }
            
            

            _lastTarget = foe.transform;
            if(_lastTarget == _target) FindTarget();
        }
    }

    Transform _lastTarget = null;

    public void FindTarget()
    {
        List<Transform> transforms = EntityManager.Instance.GetTransformsAtDistance(transform.position, _distance);
        transforms.Remove(_lastTarget);
        _target = transforms.PickRandom();
        _lastTarget = _target;
    }
}
