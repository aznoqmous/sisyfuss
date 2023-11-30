using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] Color _selectedColor;
    [SerializeField] Color _color;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] TextMeshProUGUI _nameTmp;
    [SerializeField] TextMeshProUGUI _descriptionTmp;
    [SerializeField] Animator _animator;
    ScriptableUpgrade _scriptableUpgrade;
    [SerializeField] Collider2D _collider;
    
    bool _state = false;
    bool _picked = false;
    Ball _pickedBall;
    [SerializeField] Altar _altar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        if (ball != null)
        {
            if (!Player.Instance.IsCarrying) return;
            if (_altar != null && _altar.IsPicked) return;
            collision.attachedRigidbody.velocity = Vector3.zero;
            _picked = true;
            _pickedBall = ball;
            _animator.SetBool("IsSelected", true);
            _collider.enabled = false;
            _altar.SetPicked(true);
        }
    }

    public void SetState(bool state)
    {
        if (_picked) return;

        _state = state;
        _collider.enabled = _state;
        if (state)
        {
            _nameTmp.text = $"- {_scriptableUpgrade.Name} -";
            _descriptionTmp.text = _scriptableUpgrade.Description;
        }
    }

    public void PickUp()
    {
        _pickedBall = null;
        _state = false;
        if (_altar != null) _altar.Select(this);
        Player.Instance.AddUpgrade(_scriptableUpgrade);
    }

    public void Hide()
    {
        _animator.SetBool("IsHidden", true);
    }

    private void Update()
    {
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _state || _picked ? _selectedColor : _color, Time.deltaTime * 2f);
    }

    public void SetUpgrade(ScriptableUpgrade upgrade)
    {
        _scriptableUpgrade = upgrade;
        _spriteRenderer.sprite = upgrade.Sprite;
    }
}
