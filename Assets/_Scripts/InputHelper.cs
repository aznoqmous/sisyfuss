using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHelper
{
    public static bool GetJumpDown()
    {
        return Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space);
    }

    public static bool GetActionButton()
    {
        return Input.GetKey(KeyCode.Joystick1Button2) || Input.GetMouseButton(0);
    }

    public static bool GetActionButtonDown()
    {
        return Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetMouseButtonDown(0);
    }

    public static bool GetActionButtonUp()
    {
        return Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetMouseButtonUp(0);
    }

    public static bool GetMenuButtonDown()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7);
    }
}
