using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New World", menuName="World")]
public class ScriptableWorld : ScriptableObject
{
    [Header("Generation")]
    public float LandPartsDistance = 30f;
    public float LineLength = 2f;
    public float Frequence = 10f;
    public float MinElevation = -1f;
    public float MaxElevation = 1.5f;
    public float LandPartsBeforeAltar = 3f;
    public float LandPartsBeforeBoss = 10f;

    [Header("Colors")]
    public Color SkyColor;
    public Color CloudBackColor;
    public Color CloudMiddleColor;
    public Color CloudFrontColor;
    public Color TreeColor;
    public Color FoliageColor;
    public Color StructureColor;
    public Color StructureColor2;
    public Color GroundColor;
    public Color DustColor;
    public Color FoesColor;
    public Color FoesColor2;


    [Header("Enemies")]
    public BossLandPart BossLandPart;
    public List<FoeCost> Foes;
    public Boss Boss { get { return BossLandPart.Boss;  } }

    [Header("Structures")]
    public List<GameObject> LandParts;
    public Foliage FoliagePrefab;
    public Foliage TreePrefab;

}
