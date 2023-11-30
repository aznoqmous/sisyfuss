using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    
    public bool _isColliding = false;
    public bool IsColliding { get { return _isColliding; } }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _isColliding = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _isColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isColliding = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        _isColliding = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _isColliding = false;
    }
}
