using UnityEngine;

public class Axis
{
    private float x = 0f;
    private float y = 0f;
    private float action = 0f;
    private float cancel = 0f;
    private float pause = 0f;
    
    public void SetX(float newX)
    {
        x = Mathf.Clamp(newX, -1f, 1f);
    }

    public float GetX()
    {
        return x;
    }

    public void SetY(float newY)
    {
        y = Mathf.Clamp(newY, -1f, 1f);
    }
    public float GetY()
    {
        return y;
    }

    public void SetAction(float newAction)
    {
        action = newAction;
    }  

    public float GetAction()
    {
        return action;
    }

    public void SetPause(float newPause)
    {
        pause = newPause;
    }  

    public float GetPause()
    {
        return pause;
    }

    public void SetCancel(float newCancel)
    {
        cancel = newCancel;
    }  

    public float GetCancel()
    {
        return cancel;
    }

    public void ResetAxis()
    {
        x = 0f;
        y = 0f;
        action = 0f;
    }
}
