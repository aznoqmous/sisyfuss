using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] List<Sprite> _sprites; 
    void Start()
    {
        _spriteRenderer.sprite = _sprites.PickRandom();
    }
}
