using System.Collections.Generic;
using UnityEngine;

public class InputWASDController : MonoBehaviour
{
    private const float keyboardMovementFactor = 0.7f;

    private readonly Axis axis = new();
    private Vector2 startMousePoint = Vector2.zero;
    private Vector2 startTouchPoint = Vector2.zero;
    
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
        if (Input.touches.Length == 0)
        {
            startTouchPoint = Vector2.zero;

            if (!Input.mousePresent)
            {
                return;
            }

            bool isMouseLeftPressed = Input.GetMouseButton(0);
            bool isUp = Input.GetKey(KeyCode.W);
            bool isDown = Input.GetKey(KeyCode.S);
            bool isRight = Input.GetKey(KeyCode.D);
            bool isLeft = Input.GetKey(KeyCode.A);
            bool isButtonA =
                Input.GetKey(KeyCode.Space)
                || (isMouseLeftPressed && Input.GetMouseButton(1));
            bool isButtonX = Input.GetKey(KeyCode.LeftShift) || isMouseLeftPressed;
            bool isButtonO =
                Input.GetKey(KeyCode.F)
                || Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.LeftCommand)
                || Input.GetMouseButton(1);
            bool isButtonY = Input.GetKey(KeyCode.R);

            axis.SetX((isLeft ? -1 : (isRight ? 1 : 0)) * keyboardMovementFactor);
            axis.SetY((isDown ? -1 : (isUp ? 1 : 0)) * keyboardMovementFactor);
            axis.SetButtonA(isButtonA ? 1 : 0);
            axis.SetButtonX(isButtonX ? 1 : 0);
            axis.SetButtonO(isButtonO ? 1 : 0);
            axis.SetButtonY(isButtonY ? 1 : 0);

            if (!isUp && !isDown && !isRight && !isLeft)
            {
                Vector2 mousePosition = Input.mousePosition;
                if (startMousePoint != Vector2.zero)
                {
                    var mouseButtonFactor = (isMouseLeftPressed ? 0.16f : 0.1f);
                    var axisX = mousePosition.x - startMousePoint.x;
                    var axisY = mousePosition.y - startMousePoint.y;
                    var axisSignX = axisX < 0 ? -1 : 1;
                    var axisSignY = axisY < 0 ? -1 : 1;
                    axis.SetX(Mathf.Sqrt(Mathf.Abs(axisX)) * axisSignX * mouseButtonFactor);
                    axis.SetY(Mathf.Sqrt(Mathf.Abs(axisY)) * axisSignY * mouseButtonFactor);
                }

                if (isMouseLeftPressed)
                {
                    if (Vector2.Distance(startMousePoint, mousePosition) > 50f)
                    {
                        startMousePoint = mousePosition - (mousePosition - startMousePoint).normalized * 40f;
                    }
                } else
                {
                    startMousePoint = mousePosition;
                }
            }
        } else
        {
            var touch = Input.touches[0];
            axis.SetButtonX(1);
            axis.SetButtonO(Input.touches.Length > 2 ? 1 : 0);
            axis.SetButtonA(Input.touches.Length > 3 ? 1 : 0);

            if (startTouchPoint != Vector2.zero)
            {
                var axisX = -(touch.position.x - startTouchPoint.x);
                var axisY = -(touch.position.y - startTouchPoint.y);
                var axisSignX = axisX < 0 ? 1 : -1;
                var axisSignY = axisY < 0 ? 1 : -1;

                axis.SetX(Mathf.Sqrt(Mathf.Abs(axisX)) * axisSignX * 0.16f);
                axis.SetY(Mathf.Sqrt(Mathf.Abs(axisY)) * axisSignY * 0.16f);

                if (Vector2.Distance(startTouchPoint, touch.position) > 50f)
                {
                    startTouchPoint = touch.position - (touch.position - startTouchPoint).normalized * 40f;
                }
            } else
            {
                startTouchPoint = touch.position;
            }
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
