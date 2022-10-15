using UnityEngine;
using UnityEngine.InputSystem;

public class InputGlobalController : MonoBehaviour
{
    private static Axis axis = new();

    public static Axis GetUpdatedAxis()
    {
        var axisY = 0;
        var isPause = false;
        var isAction = false;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.leftStick.y.ReadValue() > 0.2f || gamepad.dpad.y.ReadValue() == 1)
            {
                axisY = -1;
            }

            if (gamepad.leftStick.y.ReadValue() < -0.2f || gamepad.dpad.y.ReadValue() == -1)
            {
                axisY = 1;
            }

            if (gamepad.startButton.ReadValue() != 0)
            {
                isPause = true;
            }

            if (gamepad.aButton.ReadValue() != 0)
            {
                isAction = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = true;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            axisY = -1;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            axisY = 1;
        }

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Space))
        {
            isAction = true;
        }

        axis.SetY(axisY);
        axis.SetPause(isPause ? 1 : 0);
        axis.SetAction(isAction ? 1 : 0);

        return axis;
    }
}