using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] Status _statusPrefab;

    public void AddStatus(string text, Vector3 position)
    {
        Status newStatus = Instantiate(_statusPrefab, position, Quaternion.identity, transform);
        newStatus.SetText(text);
    }
}
