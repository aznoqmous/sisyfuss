using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] Transform _upgradesContainer;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] Animator _animator;

    [SerializeField] List<ScriptableUpgrade> _scriptableUpgrades = new List<ScriptableUpgrade>();
    [SerializeField] List<Upgrade> _upgrades = new List<Upgrade>();
    [SerializeField] Collider2D _collider;

    [SerializeField] List<Transform> _stones = new List<Transform>();
    List<float> _heights = new List<float>();

    [SerializeField] float _offsetY = -10f;
    public bool IsPicked { get { return _picked; } }

    bool _show = false;
    bool _isShown = false;

    private void Start()
    {
        HideUgrades();

        foreach(Upgrade upgrade in _upgrades)
        {
            ScriptableUpgrade su = _scriptableUpgrades.PickRandom();
            _scriptableUpgrades.Remove(su);
            upgrade.SetUpgrade(su);
        }

        for (int i = 0; i < _stones.Count; i++)
        {
            Vector3 position = _stones[i].position;
            position.y = TerrainManager.Instance.GetTerrainPositionAtX(_stones[i].position.x).y - 100f;
            _stones[i].position = position;
        }

    }

    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _show ? 1f : 0, Time.deltaTime * 5f);
        _upgradesContainer.localScale = Vector3.Lerp(_upgradesContainer.localScale, _show ? Vector3.one :Vector3.zero, Time.deltaTime * 5f);

        if (_selectedUpgrade != null)
        {
            _selectedUpgrade.transform.localPosition = Vector3.Lerp(_selectedUpgrade.transform.localPosition, Vector3.zero, Time.deltaTime);
        }      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null) return;
        ShowUgrades();
    }
    bool _picked = false;
    Upgrade _selectedUpgrade;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null) return;
        if (!_picked) HideUgrades();
    }
    public void SetPicked(bool state)
    {
        _picked = state;
    }

    public void Select(Upgrade upgrade)
    {
        if (_selectedUpgrade != null) return;
        foreach(Upgrade u in _upgrades)
        {
            if (u != upgrade) u.Hide();
        }
        _selectedUpgrade = upgrade;
        _picked = true;
        _collider.enabled = false;
    }

    public void ShowUgrades() {
        _show = true;
    }
    public void HideUgrades() {
        _show = false;
    }

    public void Appear()
    {
        _animator.SetTrigger("Appear");
    }

    [SerializeField] ParticleSystem _dustParticles;
    public void PlayParticles()
    {

        _dustParticles.Play();
    }
}
