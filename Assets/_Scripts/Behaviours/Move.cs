using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] float _speed = 1f;

    Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        _rigidbody.velocity = Vector2.right * transform.localScale.x;
        _rigidbody.velocity = _rigidbody.velocity.normalized * _speed;
    }
}
