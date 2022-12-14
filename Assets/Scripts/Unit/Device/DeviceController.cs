using UnityEngine;

public class DeviceController : MonoBehaviour
{
    public static int NO_DEVICE = -100;
    public static int KEYBOARD_WASD = -3;
    public static int KEYBOARD_NUMPAD = -2;
    public static Axis frozenAxis = new();

    private InputAIController inputAI;
    private InputWASDController inputWASD;
    private InputNumpadController inputNumpad;
    private InputGamepadController inputGamepad;
    
    private int deviceId = NO_DEVICE;
    private int previousDeviceId = NO_DEVICE;
    private bool isFrozen = true;

    public void Start()
    {
        inputAI = GetComponent<InputAIController>();
        inputWASD = GetComponent<InputWASDController>();
        inputNumpad = GetComponent<InputNumpadController>();
        inputGamepad = GetComponent<InputGamepadController>();
    }

    public void Update()
    {
        if (IsGamepad()) inputGamepad.UpdateAxis();
        else if (IsWASD()) inputWASD.UpdateAxis();
        else if (IsNumpad()) inputNumpad.UpdateAxis();
        else inputAI.UpdateAxis();
    }

    public void SetId(int newDeviceId)
    {
        previousDeviceId = newDeviceId;
        deviceId = newDeviceId;

        if (IsGamepad())
        {
            inputGamepad.SetGamepadId(deviceId);
        }
    }

    public int GetId()
    {
        return deviceId;
    }

    public void ResetDeviceId()
    {
        if (IsGamepad())
        {
            inputGamepad.ResetGamepadId();
        }

        deviceId = NO_DEVICE;
    }

    public bool IsEquals(int checkDeviceId)
    {
        return deviceId == checkDeviceId;
    }

    public bool IsSelected()
    {
        return deviceId != NO_DEVICE;
    }

    public Axis GetAxis()
    {
        if (isFrozen) return frozenAxis;
        if (IsGamepad()) return inputGamepad.GetAxis();
        if (IsWASD()) return inputWASD.GetAxis();
        if (IsNumpad()) return inputNumpad.GetAxis();

        return inputAI.GetAxis();
    }

    public bool IsPrevious(int checkDeviceId)
    {
        return previousDeviceId == checkDeviceId;
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    private bool IsGamepad()
    {
        return deviceId >= 0;
    }

    private bool IsWASD()
    {
        return deviceId == KEYBOARD_WASD;
    }

    private bool IsNumpad()
    {
        return deviceId == KEYBOARD_NUMPAD;
    }
}
