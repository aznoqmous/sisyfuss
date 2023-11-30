using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStep : MonoBehaviour
{
    [SerializeField] Upgrade _upgrade;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _upgrade.SetState(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _upgrade.SetState(false);
    }
}
