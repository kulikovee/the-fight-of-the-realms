using System.Collections.Generic;
using UnityEngine;

public class InputWASDController : MonoBehaviour
{
    private const float keyboardMovementFactor = 0.7f;

    private readonly Axis axis = new();
    
    private readonly static List<KeyCode> KEYBOARD_WASD_KEYS = new()
    {
        KeyCode.A,
        KeyCode.D,
        KeyCode.W,
        KeyCode.S,
        KeyCode.Space,
    };

    public Axis GetAxis()
    {
        return axis;
    }

    public void UpdateAxis()
    {
        if (Input.touches.Length <= 0)
        {
            bool isUp = Input.GetKey(KeyCode.W);
            bool isDown = Input.GetKey(KeyCode.S);
            bool isRight = Input.GetKey(KeyCode.D);
            bool isLeft = Input.GetKey(KeyCode.A);
            bool isButtonA =
                Input.GetKey(KeyCode.Space)
                || (Input.GetMouseButton(0) && Input.GetMouseButton(1));
            bool isButtonX = Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(0);
            bool isButtonO =
                Input.GetKey(KeyCode.F)
                || Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.LeftCommand)
                || Input.GetMouseButton(1);

            axis.SetX((isLeft ? -1 : (isRight ? 1 : 0)) * keyboardMovementFactor);
            axis.SetY((isDown ? -1 : (isUp ? 1 : 0)) * keyboardMovementFactor);
            axis.SetButtonA(isButtonA ? 1 : 0);
            axis.SetButtonX(isButtonX ? 1 : 0);
            axis.SetButtonO(isButtonO ? 1 : 0);

            if (!isUp && !isDown && !isRight && !isLeft)
            {
                axis.SetX(Input.GetAxis("Mouse X"));
                axis.SetY(Input.GetAxis("Mouse Y"));
            }
        } else
        {
            var touch = Input.touches[0];
            axis.SetButtonX(1);
            axis.SetButtonO(Input.touches.Length > 2 ? 1 : 0);
            axis.SetButtonA(Input.touches.Length > 3 ? 1 : 0);
            axis.SetX(touch.deltaPosition.x);
            axis.SetY(touch.deltaPosition.y);
        }
    }

    public static bool IsPressed()
    {
        foreach(var keyCode in KEYBOARD_WASD_KEYS)
        {
            if (Input.GetKeyDown(keyCode))
            {
                return true;
            }
        }

        if (Input.touches.Length > 0 || Input.GetMouseButton(0))
        {
            return true;
        }

        return false;
    }

    public static bool IsSkip()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
