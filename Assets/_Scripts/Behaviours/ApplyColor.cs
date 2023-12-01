using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyColor : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] ColorType _colorType;
    Color _color;

    private void Start()
    {
        switch (_colorType)
        {
            case ColorType.SkyColor:
                _color = GameManager.Instance.CurrentWorld.SkyColor;
                break;
            case ColorType.CloudBackColor:
                _color = GameManager.Instance.CurrentWorld.CloudBackColor;
                break;
            case ColorType.CloudMiddleColor:
                _color = GameManager.Instance.CurrentWorld.CloudMiddleColor;
                break;
            case ColorType.CloudFrontColor:
                _color = GameManager.Instance.CurrentWorld.CloudFrontColor;
                break;
            case ColorType.TreeColor:
                _color = GameManager.Instance.CurrentWorld.TreeColor;
                break;
            case ColorType.FoliageColor:
                _color = GameManager.Instance.CurrentWorld.FoliageColor;
                break;
            case ColorType.StructureColor:
                _color = GameManager.Instance.CurrentWorld.StructureColor;
                break;
            case ColorType.StructureColor2:
                _color = GameManager.Instance.CurrentWorld.StructureColor2;
                break;
            case ColorType.GroundColor:
                _color = GameManager.Instance.CurrentWorld.GroundColor;
                break;
            case ColorType.DustColor:
                _color = GameManager.Instance.CurrentWorld.DustColor;
                break;
            case ColorType.FoesColor:
                _color = GameManager.Instance.CurrentWorld.FoesColor;
                break;
            case ColorType.FoesColor2:
                _color = GameManager.Instance.CurrentWorld.FoesColor2;
                break;
            case ColorType.ChestColor:
                _color = GameManager.Instance.CurrentWorld.ChestColor;
                break;
            default:
                break;
        }

        foreach(SpriteRenderer sr in _spriteRenderers)
        {
            sr.color = _color;
        }
        if(_particleSystem)
        {
            var main = _particleSystem.main;
            main.startColor = _color;
        }
    }
    
}

public enum ColorType
{
    SkyColor,
    CloudBackColor,
    CloudMiddleColor,
    CloudFrontColor,
    TreeColor,
    FoliageColor,
    StructureColor,
    StructureColor2,
    GroundColor,
    DustColor,
    FoesColor,
    FoesColor2,
    ChestColor,
}
