using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLoot : Loot
{
    protected override void OnLoot(Player player)
    {
        player.GainCoin(1);
    }
}
