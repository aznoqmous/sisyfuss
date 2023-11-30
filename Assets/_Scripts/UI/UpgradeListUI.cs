using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeListUI : MonoBehaviour
{
    public static UpgradeListUI Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Clear();
    }

    Dictionary<ScriptableUpgrade, UpgradeUI> _uis = new Dictionary<ScriptableUpgrade, UpgradeUI>();
    [SerializeField] UpgradeUI _uiPrefab;

    public void Set(ScriptableUpgrade type, int count)
    {
        if (!_uis.ContainsKey(type)) _uis[type] = Instantiate(_uiPrefab, transform);
        _uis[type].SetSprite(type.Sprite);
        _uis[type].SetCount(count);
    }

    public void Clear()
    {
        _uis.Clear();
        foreach(Transform t in transform) Destroy(t.gameObject);
    }
}
