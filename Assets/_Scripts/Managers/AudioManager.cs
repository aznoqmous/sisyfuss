using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

            _currentTheme = _themeSource;
            _mutedTheme = _themeSource2;
        }
        else Destroy(Instance);


    }

    [SerializeField] float _themeBpm = 150f;
    [SerializeField] float _beatSync = 4f;
    [SerializeField] float _beatOffset = 32f;

    float _masterVolume = 1f;
    float _musicVolume = 1f;
    float _soundVolume = 1f;

    public float MasterValue { get { return _masterVolume;  } }
    public float MusicValue { get { return _musicVolume;  } }
    public float SoundValue { get { return _soundVolume;  } }

    [SerializeField] AudioSource _windSource;
    [SerializeField] AudioSource _themeSource;
    [SerializeField] AudioSource _themeSource2;
    [SerializeField] AudioSource _bossTheme;

    [SerializeField] AudioSource _soundSource;

    AudioSource _currentTheme;
    AudioSource _mutedTheme;
    float _currentVolume = 1f;
    float _mutedVolume = 0f;

    public float SoundVolume { get { return _soundVolume * _masterVolume; } set { } }

    private void Update()
    {
        UpdateVolumes();
    }

    public void SetTheme(AudioSource theme)
    {
        if (theme == _currentTheme) return;
 
        float beatTime = 60f / _themeBpm;
        theme.time = _currentTheme.time % (beatTime * _beatSync) + beatTime * _beatOffset;
        theme.Play();

        _mutedTheme = _currentTheme;
        _currentTheme = theme;
        _mutedVolume = 1f;
        _currentVolume = 0f;
    }

    public void PlayBossTheme()
    {
        if(_currentTheme != _bossTheme) SetTheme(_bossTheme);
    }
    public void PlayLightTheme()
    {
        if(_currentTheme != _themeSource) SetTheme(_themeSource);
    }
    public void PlayMainTheme()
    {
        if(_currentTheme != _themeSource2) SetTheme(_themeSource2);
    }
    public void PlayMutedTheme()
    {
        SetTheme(_mutedTheme);
    }

    public void SwitchTheme()
    {
        SetTheme(_mutedTheme);
    }
    public string GetActiveThemeName()
    {
        return _currentTheme.clip.name;
    }


    public void SetMasterVolume(float volume)
    {
        _masterVolume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
    }
    public void SetSoundVolume(float volume)
    {
        _soundVolume = volume;
    }

    void UpdateVolumes()
    {
        _windSource.volume = SoundVolume;
        _soundSource.volume = SoundVolume;

        _currentVolume = Mathf.Lerp(_currentVolume, 1f, Time.deltaTime);
        _mutedVolume = Mathf.Lerp(_mutedVolume, 0f, Time.deltaTime);

        _currentTheme.volume = _currentVolume * _masterVolume * _musicVolume;
        _mutedTheme.volume = _mutedVolume * _masterVolume * _musicVolume;
    }

    public void PlaySound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }
    public void PlaySoundAtPoint(AudioClip audioClip, Vector3 point)
    {
        AudioSource.PlayClipAtPoint(audioClip, point, _soundSource.volume);
    }
}
