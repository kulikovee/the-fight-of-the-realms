using UnityEngine;

public class Axis
{
    private float x = 0f;
    private float y = 0f;
    private float buttonA = 0f;
    private float buttonX = 0f;
    private float buttonO = 0f;
    private float buttonY = 0f;
    private float buttonRB = 0f;
    private float buttonLB = 0f;
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

    public void SetButtonA(float newAction)
    {
        buttonA = newAction;
    }  

    public float GetButtonA()
    {
        return buttonA;
    }

    public void SetButtonX(float newAction)
    {
        buttonX = newAction;
    }  

    public float GetButtonX()
    {
        return buttonX;
    }

    public void SetButtonY(float newAction)
    {
        buttonY = newAction;
    }

    public float GetButtonY()
    {
        return buttonY;
    }

    public void SetPause(float newPause)
    {
        pause = newPause;
    }  

    public float GetPause()
    {
        return pause;
    }

    public void SetButtonO(float newButtonO)
    {
        buttonO = newButtonO;
    }  

    public float GetButtonO()
    {
        return buttonO;
    }

    public void SetButtonRB(float newButtonRB)
    {
        buttonRB = newButtonRB;
    }  

    public float GetButtonRB()
    {
        return buttonRB;
    }

    public void SetButtonLB(float newButtonLB)
    {
        buttonLB = newButtonLB;
    }  

    public float GetButtonLB()
    {
        return buttonLB;
    }

    public void ResetAxis()
    {
        x = 0f;
        y = 0f;
        buttonA = 0f;
        buttonX = 0f;
        buttonO = 0f;
        buttonY = 0f;
        pause = 0f;
    }
}
