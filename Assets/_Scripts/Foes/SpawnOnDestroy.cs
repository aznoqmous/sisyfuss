using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] List<Foe> _spawnedFoes = new List<Foe>();

    private void OnDestroy()
    {
        Debug.Log("DESTROY " + _spawnedFoes);
        foreach (Foe foe in _spawnedFoes) EntityManager.Instance.SpawnFoe(foe, transform.position);
    }
}
