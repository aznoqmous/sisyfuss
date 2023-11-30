using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkMovement : Movement
{
    [SerializeField] Transform _raycastOriginFront;
    [SerializeField] Transform _raycastOriginBack;
    public bool IsGrounded { get { return _isGrounded; } }
    bool _isGrounded;
    int _layerMask;
    float _gravityScale;
    bool _isJumping = false;

    void Start()
    {
        _layerMask = LayerMask.GetMask("Walkable");
        _gravityScale = _rigidBody.gravityScale;
    }

    void FixedUpdate()
    {
        if (_foe.CanMove) Move();
    }

    public void Jump(Vector2 force)
    {
        _isJumping = true;
        _rigidBody.AddForce(force);
    }

    override public void AddForce(Vector2 force)
    {
        _isJumping = true;
        _rigidBody.AddForce(force);
    }

    void Move()
    {
        RaycastHit2D hitFront = Physics2D.Raycast(_raycastOriginFront.position, -Vector2.up, 1f, _layerMask);
        RaycastHit2D hitBack = Physics2D.Raycast(_raycastOriginBack.position, -Vector2.up, 1f, _layerMask);
        _isGrounded = hitFront.collider != null && hitBack.collider != null;
        if (!_isGrounded) _isJumping = false;
        if (_isGrounded && !_isJumping)
        {
            Vector2 normal = hitFront.point - hitBack.point;
            float angle = Mathf.Abs(Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg) - 90; //get angle
            float scaleX = Mathf.Sign((transform.position - Player.Instance.transform.position).x);
            _spriteRendererContainer.localEulerAngles = new Vector3(0, 0, angle);
            _spriteRendererContainer.localScale = new Vector3(scaleX, 1, 1);
            //Vector2 movement = new Vector2(Mathf.Sin(angle + Mathf.PI/2), Mathf.Cos(angle + Mathf.PI/2));
            Vector2 movement = (Player.Instance.transform.position - transform.position).normalized * _moveSpeed * Time.deltaTime * 10f;
            movement.y = _rigidBody.velocity.y;
            _rigidBody.velocity = movement;

        }

        _rigidBody.gravityScale = _isGrounded ? 1f : _gravityScale;
    }
}
