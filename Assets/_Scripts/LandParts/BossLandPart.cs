using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLandPart : MonoBehaviour
{
    [SerializeField] Boss _boss;
    [SerializeField] Collider2D _startBattleCollider;
    public Boss Boss { get { return _boss; } }

    private void Start()
    {
        _boss.SetIdle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null) {
            _boss.SetIdle(false);
            _startBattleCollider.enabled = false;
            AudioManager.Instance.PlayBossTheme();
        }
    }
}
