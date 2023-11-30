using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    public static SettingsPanelUI Instance;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Toggle();
    }


    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        if (gameObject.activeInHierarchy) GameManager.Instance.Pause();
        else GameManager.Instance.Play();
    }


}
