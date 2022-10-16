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

    public Axis GetUpdatedAxis()
    {
        UpdateAxis();
        return axis;
    }
    void UpdateAxis()
    {
        bool isUp = Input.GetKey(KeyCode.W);
        bool isDown = Input.GetKey(KeyCode.S);
        bool isRight = Input.GetKey(KeyCode.D);
        bool isLeft = Input.GetKey(KeyCode.A);
        bool isAction = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1);
        bool isAction2 = Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(0);

        axis.SetX((isLeft ? -1 : (isRight ? 1 : 0)) * keyboardMovementFactor);
        axis.SetY((isDown ? -1 : (isUp ? 1 : 0)) * keyboardMovementFactor);
        axis.SetAction(isAction ? 1 : 0);
        axis.SetAction2(isAction2 ? 1 : 0);
    }

    public static bool IsPressed()
    {
        var isWasd = false;

        KEYBOARD_WASD_KEYS.ForEach((keyCode) => {
            if (Input.GetKeyDown(keyCode))
            {
                isWasd = true;
            }
        });

        return isWasd;
    }

    public static bool IsSkip()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
