using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float _spawnTimeout = 5f;
    [SerializeField] GameObject _spawnPrefab;
    [SerializeField] Transform _spawnTransform;

    float _lastSpawn;

    private void Update()
    {
        if(Time.time - _lastSpawn > _spawnTimeout)
        {
            Spawn();
            _lastSpawn = Time.time;
        }    
    }

    void Spawn() {
        Instantiate(_spawnPrefab, _spawnTransform.position, Quaternion.identity);
    }
}
