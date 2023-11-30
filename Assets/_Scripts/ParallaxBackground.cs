using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    Vector3 _lastCameraPosition;
    [SerializeField] Vector2 _speed = Vector2.zero;
    [SerializeField] SpriteRenderer _spriteRenderer;
    float _textureUnitSizeX;

    Transform _cameraTransform;
    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;
        Sprite sprite = _spriteRenderer.sprite;
        Texture2D texture = sprite.texture;
        _textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {

        Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
        transform.position -= new Vector3(deltaMovement.x * _speed.x + Time.deltaTime * 5f * _speed.x, deltaMovement.y * _speed.y, deltaMovement.z);
        _lastCameraPosition = _cameraTransform.position;

        if(Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
        {
            float offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
            transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
    }
}
