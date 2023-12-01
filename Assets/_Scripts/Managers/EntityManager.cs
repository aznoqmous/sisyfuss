using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    List<FoeCost> _foes = new List<FoeCost>();
    List<Foe> _foesList = new List<Foe>();
    float _money = 0f;
    public float Money { get { return _money;  } }
    float _nextSpawn = 0f;
    [SerializeField] float _minSpawnTimeout = 10f;
    [SerializeField] float _maxSpawnTimeout = 10f;
    [SerializeField] float _minRetrySpawnTimeout = 1f;
    [SerializeField] float _maxRetrySpawnTimeout = 1f;
    [SerializeField] float _minSpawnDistance = 30f;
    [SerializeField] float _maxSpawnDistance = 40f;

    float _spawnTimeout { get { return UnityEngine.Random.Range(_minSpawnTimeout, _maxSpawnTimeout); } }
    float _retrySpawnTimeout { get { return UnityEngine.Random.Range(_minRetrySpawnTimeout, _maxRetrySpawnTimeout); } }

    Boss _boss;
    public bool IsBossAlive { get { return _boss != null && !_boss.IsDead; } }
    public bool IsBossIdle { get { return _boss != null && _boss.IsIdle; } }

    private void Update()
    {
        _money += Time.deltaTime * GameManager.Instance.Difficulty / 2f;
        HandleSpawn();
    }

    void HandleSpawn()
    {
        if (Time.time > _nextSpawn)
        {
            _nextSpawn = Time.time;
            FoeCost foeCost = PickAvailableFoe();
            if (foeCost != null)
            {
                Foe newFoe = GenerateFoe(foeCost.Foe);
                _money -= foeCost.Cost;
                _nextSpawn += _retrySpawnTimeout;

                SpawnImprove(newFoe);
            }
            else
            {
                _nextSpawn += _spawnTimeout;
            }
        }
    }

    Foe SpawnImprove(Foe foe)
    {
        FoeCost foeCost = PickAvailableFoe();
        if (foeCost != null)
        {
            foe.ConnectTo(GenerateFoe(foeCost.Foe, foe.transform.position + Vector3.right * 2f));
            _money -= foeCost.Cost;
            _nextSpawn += _retrySpawnTimeout;
            return foe;
        }
        return null;
    }

    public Foe GenerateFoe(Foe foePrefab, Vector3? position=null)
    {
        Vector3 pos = Vector3.zero;
        if (position.HasValue) pos = position.Value;
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(Player.Instance.transform.position + Vector3.up * 10f + Vector3.right * UnityEngine.Random.Range(_minSpawnDistance, _maxSpawnDistance), Vector2.down);
            if (hit != null)
            {
                pos = (Vector3) hit.point + Vector3.up;
            }
            else
            {
                float x = 10f + Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Player.Instance.transform.position.x;
                pos = Player.Instance.transform.position + (new Vector3(x, 20f, 0f));
            }
        }

        return SpawnFoe(foePrefab, pos);
    }

    public Foe SpawnFoe(Foe foePrefab, Vector3 position)
    {
        Foe foe = Instantiate(foePrefab, position, Quaternion.identity, transform);
        return foe;
    }

    public void Add(Foe foe)
    {
        _foesList.Add(foe);
    }
    public void Remove(Foe foe)
    {
        _foesList.Remove(foe);
    }

    FoeCost PickAvailableFoe()
    {
        List<FoeCost> _availableFoes = _foes.Where((FoeCost fc) => fc.Enabled && fc.Cost <= _money).ToList();
        return _availableFoes.PickRandom();
    }

    public List<Transform> GetTransformsAtDistance(Vector3 position, float distance)
    {
        List<Transform> transforms = new List<Transform>();
        foreach(Transform t in transform)
        {
            if (t.position.DistanceTo(position) <= distance) transforms.Add(t);
        }
        return transforms;
    }
    public Transform GetNearestTransform(Vector3 position)
    {
        float minDistance = -999f;
        Transform nearest = null;
        foreach (Transform t in transform)
        {
            float distance = t.position.DistanceTo(position);
            if (distance <= minDistance || minDistance < 0)
            {
                nearest = t;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public Foe GetNearestFoe(Vector3 position)
    {
        float minDistance = -999f;
        Foe nearest = null;
        List<Foe> list = new List<Foe>(_foesList);

        foreach (Foe foe in list)
        {
            if(foe == null)
            {
                _foesList.Remove(foe);
                continue;
            }
            float distance = foe.transform.position.DistanceTo(position);
            if (distance <= minDistance || minDistance < 0)
            {
                nearest = foe;
                minDistance = distance;
            }
        }
        return nearest;
    }


    public void SetBoss(Boss boss)
    {
        _boss = boss;
    }

    public void SetFoes(List<FoeCost> foes)
    {
        _foes = foes;
    }
}

[Serializable]
public class FoeCost
{
    public bool Enabled;
    public float Cost;
    public Foe Foe;
}