using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBehaviour : MonoBehaviour
{
    [SerializeField] bool _enabled = true;
    [SerializeField] float _successTimeout = 1f;
    [SerializeField] float _retryTimeout = 1f;
    [SerializeField] float _chance = 0.1f;
    [SerializeField] float _nextTryTime = 1f;
    [SerializeField] Foe _foe;

    private void Update()
    {
        if(_foe.IsDead) return;
        if (Enabled() && Time.time > _nextTryTime)
        {
            if (Random.value < _chance)
            {
                Do();
                _nextTryTime = Time.time + _successTimeout;
            }
            else
            {
                _nextTryTime = Time.time + _retryTimeout;
            }
        }
    }

    virtual protected void Do()
    {

    }

    virtual protected bool Enabled()
    {
        return _enabled;
    }
}
