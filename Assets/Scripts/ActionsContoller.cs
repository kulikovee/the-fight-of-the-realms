using System;
using System.Collections;
using UnityEngine;

public class ActionsContoller : MonoBehaviour
{
    public delegate void VoidDelegate();
    public delegate void IntDelegate(int value);
    public delegate void FourBoolDelegate(bool value1, bool value2, bool value3, bool value4);
    public delegate void TwoUnitDelegate(UnitController dead, UnitController killer);

    public static event VoidDelegate OnRoundEnd;
    public static event VoidDelegate OnRoundStart;
    public static event VoidDelegate OnShowStartupMenu;
    public static event VoidDelegate OnFirstShowStartupMenu;
    public static event VoidDelegate OnResetPlayersText;
    public static event VoidDelegate OnJoinedPlayersText;
    public static event VoidDelegate OnEndGame;
    public static event VoidDelegate OnStartGame;
    public static event IntDelegate OnTimerUpdate;
    public static event IntDelegate OnSelectPauseOption;
    public static event FourBoolDelegate OnUpdateScore;
    public static event TwoUnitDelegate OnUnitKilled;

    public static ActionsContoller GetActions()
    {
        return GameObject.FindObjectOfType<ActionsContoller>();
    }

    public void RoundRestart()
    {
        RoundEnd();
        StartCoroutine(StartRoundAfterDelay());
    }

    IEnumerator StartRoundAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        RoundStart();
    }

    public void RoundEnd()
    {
        OnRoundEnd?.Invoke();
    }

    public void UnitKilled(UnitController dead, UnitController killer)
    {
        OnUnitKilled?.Invoke(dead, killer);
    }
    public void RoundStart()
    {
        OnRoundStart?.Invoke();
    }

    public void ShowStartupMenu()
    {
        OnShowStartupMenu?.Invoke();
    }

    public void FirstShowStartupMenu()
    {
        OnFirstShowStartupMenu?.Invoke();
    }

    public void ResetPlayersText()
    {
        OnResetPlayersText?.Invoke();
    }

    public void JoinedPlayersText()
    {
        OnJoinedPlayersText?.Invoke();
    }
    
    public void EndGame()
    {
        OnEndGame?.Invoke();
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
    }

    public void TimerUpdate(int seconds)
    {
        OnTimerUpdate?.Invoke(seconds);
    }

    public void SelectPauseOption(int option)
    {
        OnSelectPauseOption?.Invoke(option);
    }

    public void UpdateScore(bool isFirstPlayerScore, bool isSecondPlayerScore, bool isThirdPlayerScore, bool isForthPlayerScore)
    {
        OnUpdateScore?.Invoke(isFirstPlayerScore, isSecondPlayerScore, isThirdPlayerScore, isForthPlayerScore);
    }
}
