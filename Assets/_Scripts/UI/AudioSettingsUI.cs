using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _soundSlider;
    [SerializeField] Button _switchThemeButton;
    [SerializeField] TextMeshProUGUI _switchThemeButtonText;

    private void Start()
    {
        _masterSlider.value = AudioManager.Instance.MasterValue;
        _musicSlider.value = AudioManager.Instance.MusicValue;
        _soundSlider.value = AudioManager.Instance.SoundValue;

        _masterSlider.onValueChanged.AddListener(delegate { AudioManager.Instance.SetMasterVolume(_masterSlider.value); });
        _musicSlider.onValueChanged.AddListener(delegate { AudioManager.Instance.SetMusicVolume(_musicSlider.value); });
        _soundSlider.onValueChanged.AddListener(delegate { AudioManager.Instance.SetSoundVolume(_soundSlider.value); });

        _switchThemeButton.onClick.AddListener(delegate
        {
            AudioManager.Instance.SwitchTheme();
        });
    }

    void Update() {
        _switchThemeButtonText.text = $"Switch theme ({AudioManager.Instance.GetActiveThemeName()})";
    }
}
