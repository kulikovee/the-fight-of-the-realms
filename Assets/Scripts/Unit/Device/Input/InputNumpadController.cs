using System.Collections.Generic;
using UnityEngine;

public class InputNumpadController : MonoBehaviour
{
    private const float keyboardMovementFactor = 0.7f;
    
    private readonly Axis axis = new();

    public static List<KeyCode> KEYBOARD_ARROWS_KEYS = new()
    {
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad8,
        KeyCode.Keypad0,
    };

    public Axis GetAxis()
    {
        return axis;
    }

    public void UpdateAxis()
    {
        bool isUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Keypad8);
        bool isDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Keypad5);
        bool isRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Keypad6);
        bool isLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Keypad4);
        bool isButtonX = Input.GetKey(KeyCode.Keypad0)
            || Input.GetKey(KeyCode.RightCommand)
            || Input.GetKey(KeyCode.RightApple)
            || Input.GetKey(KeyCode.RightShift);
        bool isButtonA = Input.GetKey(KeyCode.Keypad1);
        bool isButtonO = Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.Keypad7);

        axis.SetX((isLeft ? -1 : (isRight ? 1 : 0)) * keyboardMovementFactor);
        axis.SetY((isDown ? -1 : (isUp ? 1 : 0)) * keyboardMovementFactor);
        axis.SetButtonA(isButtonA ? 1 : 0);
        axis.SetButtonX(isButtonX ? 1 : 0);
        axis.SetButtonO(isButtonO ? 1 : 0);
    }

    public static bool IsPressed()
    {
        var isPressed = false;

        KEYBOARD_ARROWS_KEYS.ForEach((keyCode) => {
            if (Input.GetKeyDown(keyCode))
            {
                isPressed = true;
            }
        });

        return isPressed;
    }
}
