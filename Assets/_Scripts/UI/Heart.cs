using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] Image _image;

    bool _state = false;

    public void SetActive(bool state = true)
    {
        _state = state;
    }

    private void Update()
    {
        _image.transform.localScale = Vector3.Lerp(_image.transform.localScale, _state ? Vector3.one : Vector3.zero, Time.unscaledDeltaTime * 10f);
    }
}
