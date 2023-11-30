using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeChest : Chest
{
    [SerializeField] List<ScriptableUpgrade> _upgrades= new List<ScriptableUpgrade>();

    [SerializeField] CanvasGroup _costGroup;
    [SerializeField] TextMeshProUGUI _costText;

    [SerializeField] CanvasGroup _upgradeGroup;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _descriptionText;

    float _cost = 10f;

    public override bool CanOpen { get { return Player.Instance.Coins >= _cost; } }

    private void Start()
    {

        if(!TerrainManager.Instance.CanSpawnUpgradeChest)
        {
            Destroy(gameObject);
            return;
        }
        TerrainManager.Instance.UpgradeChestCount++;
        SetCost(TerrainManager.Instance.TotalLandPartCount+3f);
    }
    private void Update()
    {
        if (_isOpened)
        {
            SetCost(Mathf.Lerp(_cost, 0, Time.deltaTime * 2f));
            _upgradeGroup.alpha = Mathf.Lerp(_upgradeGroup.alpha, 1, Time.deltaTime * 2f);
            _costGroup.alpha = Mathf.Lerp(_costGroup.alpha, 0, Time.deltaTime * 2f);
        }
    }

    protected override void OnOpen()
    {
        Player.Instance.LoseCoin(_cost);
        ScriptableUpgrade upgrade = _upgrades.PickRandom();
        _image.sprite = upgrade.Sprite;
        _titleText.text = upgrade.Name;
        _descriptionText.text = upgrade.Description;

        Player.Instance.AddUpgrade(upgrade);
    }

    public void SetCost(float amount)
    {
        _cost = amount;
        _costText.text = Mathf.FloorToInt(_cost).ToString();
    }
}
