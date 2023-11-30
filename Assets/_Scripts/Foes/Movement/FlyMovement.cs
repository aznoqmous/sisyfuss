using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMovement : Movement
{
    public float PlayerDistance = 1f;
    public float GroundDistance = 1f;

    public bool IsIdle = false;
    int _layerMask;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Walkable");
    }

    private void Update()
    {
        IsIdle = Player.Instance.transform.position.DistanceTo(_rigidBody.transform.position) < PlayerDistance;
    }

    protected override void Move()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(_rigidBody.transform.position, Vector2.down, Mathf.Infinity, _layerMask);
        
        Vector3 target = IsIdle ? _rigidBody.transform.position : Player.Instance.transform.position;
        
        target.y = raycastHit.point.y + GroundDistance;

        if (_rigidBody.transform.position.DistanceTo(target) < 1f) return;

        if(raycastHit.collider!= null)
        {
            Ball ball = raycastHit.collider.GetComponent<Ball>();
            if (ball != null )
            {
                target.y -= ball.Size;
            }
        }

        //Debug.DrawLine(_rigidBody.transform.position, (Vector2)_rigidBody.transform.position + Vector2.down * GroundDistance, Color.red);

        float scaleX = Mathf.Sign((_rigidBody.transform.position - Player.Instance.transform.position).x);
        _spriteRendererContainer.localScale = new Vector3(scaleX, 1, 1);

        Vector2 movement = (target - _rigidBody.transform.position).normalized * _moveSpeed * Time.deltaTime / 10f;
        if (IsIdle) movement.x = 0;
        _rigidBody.MovePosition(_rigidBody.transform.position + (Vector3) movement);
        _rigidBody.velocity = Vector2.zero;
    }
}
