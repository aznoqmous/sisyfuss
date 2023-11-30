using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthLoot : Loot
{
    protected override void OnLoot(Player player)
    {
        player.Heal(1);
    }
}
