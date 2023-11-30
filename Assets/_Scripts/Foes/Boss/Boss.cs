using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected Foe _foe;

    protected bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    public bool IsIdle = true;

    public void SetIdle(bool state = true)
    {
        IsIdle = state;
        if (state) _foe.PreventMovement();
        else
        {
            _foe.EnableMovement();
            OnActivate();
        }
    }

    private void Update()
    {
        if (IsIdle) return;

        if (_isDead) return;
        if (!_isDead && _foe.IsDead)
        {
            _isDead = true;
            EntityManager.Instance.SetBoss(null);
            AudioManager.Instance.PlayLightTheme();
            OnDeath();
            return;
        }

        Cast();
    }

    protected virtual void Cast()
    {

    }

    protected virtual void OnDeath()
    {

    }
    protected virtual void OnActivate()
    {

    }
}
