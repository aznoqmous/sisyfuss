using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tmp;

    public void SetText(string text)
    {
        _tmp.text = text;
    }

    public void Erase()
    {
        Destroy(gameObject);
    }
}
