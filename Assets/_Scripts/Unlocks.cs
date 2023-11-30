using System;

[Serializable]
public class Unlocks
{
    public bool BouncingBall = false;
    public bool FloatingBall = false;

    public void Unlock(UnlockType unlockType)
    {
        if (IsUnlocked(unlockType)) return;
        switch (unlockType)
        {
            case UnlockType.BouncingBall:
                BouncingBall= true;
                break;
            case UnlockType.FloatingBall:
                FloatingBall= true;
                break;
            default:
                break;
        }
        GameManager.Instance.DisplayUnlockMessage();
        Data.SaveUnlocks(this);
    }

    public bool IsUnlocked(UnlockType unlockType)
    {
        switch (unlockType)
        {
            case UnlockType.None:
                return true;
            case UnlockType.BouncingBall:
                return BouncingBall;
            case UnlockType.FloatingBall:
                return FloatingBall;
            default:
                break;
        }
        return false;
    }
}

public enum UnlockType
{
    None,
    BouncingBall,
    FloatingBall
}
