using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHolder : MonoBehaviour
{
    [SerializeField] Ball _ball;
    [SerializeField] Transform _ballContainer;
    Vector3 _targetPosition;

    bool _isActive = false;

    private void Start()
    {
        _isActive = _ball.IsUnlocked;
        _ball.gameObject.SetActive(_isActive);
        if (!_isActive)
        {
            Destroy(gameObject);
            return;
        }
    

        _targetPosition = _ballContainer.transform.position + Vector3.up * _ball.Size / 2;
        if (Player.Instance.Ball != null && _ball.name == Player.Instance.Ball.name) _ball = Player.Instance.Ball;
        else
        {
            _ball = Instantiate(_ball, _targetPosition, Quaternion.identity, _ballContainer);
            _ball.EnablePhysics(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!_isActive || _ball.IsAttached) return;
        if(collision.GetComponent<Player>() != null)
        {
            Player.Instance.SelectBall(_ball);
            
        }
    }

    private void Update()
    {
        if (!_isActive || _ball == null || _ball.IsAttached) return;
        _ball.transform.position = Vector3.Lerp(_ball.transform.position, _ballContainer.transform.position + Vector3.up * _ball.Size, Time.deltaTime * 10f);
    }
}
