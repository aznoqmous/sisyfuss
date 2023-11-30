using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected HealthLoot _healthLootPrefab;
    [SerializeField] protected CoinLoot _coinLootPrefab;
    [SerializeField] protected Animator _animator;
    [SerializeField] AudioClip _openClip;

    public virtual bool CanOpen { get { return true; } }
    protected bool _isOpened = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null && collision.GetComponent<Ball>() == null) return;
        if (!CanOpen) return;
        _isOpened = true;
        _animator.SetBool("IsOpen", true);
        OnOpen();
        _collider.enabled = false;
        AudioManager.Instance.PlaySound(_openClip);
    }

    protected void SpawnLoot(Loot loot)
    {
        Loot newLoot = Instantiate(loot, transform.position + Vector3.up, Quaternion.identity);
        newLoot.AddForce(Vector2.up.Rotate(Random.Range(-20f, 20f)) * 400f);
    }

    protected virtual void OnOpen()
    {
        if (LootManager.Instance.PlayerMissingHealthRandom(0f, 1f))
        {
            SpawnLoot(_healthLootPrefab);
        }
        else
        {
            SpawnLoot(_coinLootPrefab);
            SpawnLoot(_coinLootPrefab);
            SpawnLoot(_coinLootPrefab);
        }
    }
}
