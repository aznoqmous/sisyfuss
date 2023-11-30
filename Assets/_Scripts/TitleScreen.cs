using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] AudioSource _theme;
    [SerializeField] AudioSource _stoneDrop;
    [SerializeField] AudioSource _rockDrop;

    public void PlayTheme()
    {
        _theme.Play();
    }
    public void PlayStoneDrop()
    {
        _stoneDrop.Play();
    }
    public void PlayRockDrop()
    {
        _rockDrop.Play();
    }
    public void Play()
    {
        _played = true;
        LevelManager.Instance.LoadScene("LevelScene");
    }

    bool _played = false;
    private void Update()
    {
        if (_played) _theme.volume = Mathf.Lerp(_theme.volume, 0f, Time.deltaTime * 2f);
    }
}
