using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollide : MonoBehaviour
{
    [SerializeField] AudioClip _audio;
    [SerializeField] bool _playOnce = false;

    bool _played = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playOnce && _played) return;
        AudioManager.Instance.PlaySound(_audio);
        _played = true;
    }
}
