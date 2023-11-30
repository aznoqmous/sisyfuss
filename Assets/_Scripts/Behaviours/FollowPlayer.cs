using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] Rigidbody2D _rigidBody;

    bool _isGrounded;
    int _layerMask;
    float _gravityScale;

    void Start()
    {
        _layerMask = LayerMask.GetMask("Terrain");
        Debug.Log(_layerMask);
        _gravityScale = _rigidBody.gravityScale;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, _layerMask);
        _isGrounded = hit.collider != null;

        _rigidBody.gravityScale = _isGrounded ? 1f : _gravityScale;

        Vector2 movement = _rigidBody.velocity;
        movement.x = (Player.Instance.transform.position - transform.position).normalized.x * _moveSpeed * Time.deltaTime;
        _rigidBody.velocity = movement;
    }
}
