using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{

    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _countTmp;

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetCount(int count)
    {
        _countTmp.text = count.ToString();
        if (count <= 1) _countTmp.text = "";
    }
}
