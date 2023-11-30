using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 1f;
    [SerializeField] protected Rigidbody2D _rigidBody;
    [SerializeField] protected Foe _foe;
    [SerializeField] protected Transform _spriteRendererContainer;

    void FixedUpdate()
    {
        if (_foe.CanMove) Move();
    }

    protected virtual void Move()
    {
        Vector3 target = Player.Instance.transform.position;
        float scaleX = Mathf.Sign((transform.position - target).x);
        _spriteRendererContainer.localScale = new Vector3(scaleX, 1, 1);
        Vector2 movement = (target - transform.position).normalized * _moveSpeed * Time.deltaTime / 10f;
        _rigidBody.MovePosition(transform.position + (Vector3)movement);
    }

    public virtual void AddForce(Vector2 force)
    {
        _rigidBody.AddForce(force);
    }
}
