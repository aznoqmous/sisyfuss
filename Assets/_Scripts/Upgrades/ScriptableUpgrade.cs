using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Upgrade", menuName ="Upgrade")]
public class ScriptableUpgrade : ScriptableObject
{
    public Sprite Sprite;
    public UpgradeType Type;
    public string Name;
    public string Description;
}

public enum UpgradeType
{
    AirThunder,
    BallSize,
    MaxJumps,
    MoveSpeed,
    RollFire,
    Roping,
    Throwing,

    MaxHealth,
    LinkLength,
    FasterChargeTime,

    JumpShot,
    LinkDamage,
    StoneLaser,


    MegaThrow,

}