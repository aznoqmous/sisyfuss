using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Foe foe = collision.GetComponent<Foe>();
        if (foe != null) foe.TakeDamage(Player.Instance.LinkDamage);
    }
}
