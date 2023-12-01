using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public static MousePointer Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        Cursor.visible = false;

    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetState(bool state)
    {
        gameObject.SetActive(state);
    }
}
