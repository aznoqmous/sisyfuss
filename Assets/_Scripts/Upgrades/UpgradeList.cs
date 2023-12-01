using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeList
{

    public int AirThunder = 0;
    public int BallSize = 0;
    public int MaxJumps = 0;
    public int MegaThrow = 0;
    public int MoveSpeed = 0;
    public int RollFire = 0;
    public int Roping = 0;
    public int Throwing = 0;
    public int MaxHealth = 0;
    public int LinkLength = 0;
    public int FasterChargeTime = 0;
    public int JumpShot = 0;
    public int LinkDamage = 0;
    public int StoneLaser = 0;

    public int Gravity = 0;
    public int Quake = 0;
    public int Trail = 0;

    Dictionary<ScriptableUpgrade, int> _upgrades = new Dictionary<ScriptableUpgrade, int>();
    public void AddUpgrade(ScriptableUpgrade su)
    {
        if (!_upgrades.ContainsKey(su)) _upgrades.Add(su, 0);
        _upgrades[su]++;
        UpgradeListUI.Instance.Set(su, _upgrades[su]);

        switch (su.Type)
        {
            case UpgradeType.AirThunder:
                AirThunder++;
                break;
            case UpgradeType.BallSize:
                BallSize++;
                break;
            case UpgradeType.MaxJumps:
                MaxJumps++;
                break;
            case UpgradeType.MegaThrow:
                MegaThrow++;
                break;
            case UpgradeType.MoveSpeed:
                MoveSpeed++;
                break;
            case UpgradeType.RollFire:
                RollFire++;
                break;
            case UpgradeType.Roping:
                Roping++;
                break;
            case UpgradeType.Throwing:
                Throwing++;
                break;
            case UpgradeType.MaxHealth:
                MaxHealth++;
                break;
            case UpgradeType.LinkLength:
                LinkLength++;
                break;
            case UpgradeType.FasterChargeTime:
                FasterChargeTime++;
                break;
            case UpgradeType.JumpShot:
                JumpShot++;
                break;
            case UpgradeType.LinkDamage:
                LinkDamage++;
                break;
            case UpgradeType.StoneLaser:
                StoneLaser++;
                break;
            case UpgradeType.Gravity:
                Gravity++;
                break;
            case UpgradeType.Quake:
                Quake++;
                break;
            case UpgradeType.Trail:
                Trail++;
                break;
            default:
                break;
        }

    }


}
