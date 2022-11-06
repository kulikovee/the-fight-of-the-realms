using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputGamepadController : MonoBehaviour
{
    const int NOT_SELECTED = -1;

    readonly Axis axis = new();
    int gamePadId = NOT_SELECTED;

    public Axis GetAxis()
    {
        return axis;
    }

    public void SetGamepadId(int newGamePadId)
    {
        gamePadId = newGamePadId;
    }

    public void ResetGamepadId()
    {
        gamePadId = NOT_SELECTED;
    }

    public void UpdateAxis()
    {
        if (gamePadId < 0 || gamePadId > Gamepad.all.Count - 1)
        {
            return;
        }

        var gamePad = Gamepad.all[gamePadId];
        var stickX = gamePad.leftStick.x.ReadValue();
        var stickY = gamePad.leftStick.y.ReadValue();

        if (Math.Abs(stickX) > 0.1f || Math.Abs(stickY) > 0.1f)
        {
            axis.SetX(stickX);
            axis.SetY(stickY);
        }
        else
        {
            axis.SetX(gamePad.dpad.x.ReadValue());
            axis.SetY(gamePad.dpad.y.ReadValue());
        }

        axis.SetButtonA(gamePad.buttonSouth.ReadValue());
        axis.SetButtonX(gamePad.buttonWest.ReadValue());
        axis.SetButtonO(gamePad.buttonEast.ReadValue());
        axis.SetButtonY(gamePad.buttonNorth.ReadValue());
        axis.SetButtonLB(gamePad.leftShoulder.ReadValue());
        axis.SetButtonRB(gamePad.rightShoulder.ReadValue());
    }

    public static List<int> GetPressedIds()
    {
        var pressedIds = new List<int>();

        for (int gamePadId = 0; gamePadId < Gamepad.all.Count; gamePadId++)
        {
            var gamePad = Gamepad.all[gamePadId];
            if (
                gamePad.aButton.isPressed
                || gamePad.bButton.isPressed
                || gamePad.yButton.isPressed
                || gamePad.xButton.isPressed
            )
            {
                pressedIds.Add(gamePadId);
            }
        }

        return pressedIds;
    }

    public static List<int> GetSkipGamepadIds()
    {
        var skipGamePadIds = new List<int>();

        for (int gamePadId = 0; gamePadId < Gamepad.all.Count; gamePadId++)
        {
            if (Gamepad.all[gamePadId].startButton.isPressed)
            {
                skipGamePadIds.Add(gamePadId);
            }
        }

        return skipGamePadIds;
    }
}
