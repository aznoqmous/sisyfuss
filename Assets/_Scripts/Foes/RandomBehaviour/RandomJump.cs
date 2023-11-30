using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomJump : RandomBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    
    [SerializeField] Vector2 _jumpForce;
    [SerializeField] WalkMovement _walkMovement;

    override protected void Do()
    {
        _walkMovement.Jump(new Vector2(Mathf.Sign(_rigidBody.velocity.x), 1) * _jumpForce * 10f);
        //_walkMovement.Jump(Vector2.up * _jumpForce);
    }

    override protected bool Enabled()
    {
        return base.Enabled() && _walkMovement.IsGrounded;
    }
}
