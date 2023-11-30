using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingRoom : MonoBehaviour
{
    [SerializeField] List<Ball> _balls;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] SpriteRenderer _groundSprite;

    public void HideLineRenderer()
    {
        _lineRenderer.enabled = false;
        _groundSprite.enabled = false;
    }

    private void Start()
    {
        
    }
}
