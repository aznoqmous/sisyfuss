using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarAppear : MonoBehaviour
{
    [SerializeField] Altar _altar;
    bool _appeared = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_appeared) return;
        _altar.Appear();
        _appeared = true;
    }
}
