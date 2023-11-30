using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] HealthLoot _healthLoot;
    [SerializeField] CoinLoot _coinLoot;

    public CoinLoot SpawnCoin(Vector3 position, Vector3? force=null)
    {
        CoinLoot coinLoot = Instantiate(_coinLoot, position + Vector3.up * 2f, Quaternion.identity, transform);
        if(force.HasValue) coinLoot.AddForce(force.Value);
        return coinLoot;
    }

    public void SpawnCoins(Vector3 position, float count=1, float force=0) {
        for(int i = 0; i < count; i++)
        {
            if(Random.value <= count - i) SpawnCoin(position, Vector2.up.Rotate(Random.Range(-1f * force, force)) * 200f);
        }
    }

    public HealthLoot SpawnHeart(Vector3 position)
    {
        return Instantiate(_healthLoot, position + Vector3.up * 2f, Quaternion.identity, transform);
    }

    /**
     * Random roll increased with player missing health
     * */
    public bool PlayerMissingHealthRandom(float min, float max)
    {
        return Random.value < Mathf.Lerp(min, max, 1f - (Player.Instance.CurrentHealth-1) / (Player.Instance.MaxHealth-1));
    }
}
