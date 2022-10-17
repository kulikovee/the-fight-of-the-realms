using System;
using System.Collections;
using UnityEngine;

public class ActionsContoller : MonoBehaviour
{
    public delegate void VoidDelegate();
    public delegate void IntDelegate(int value);
    public delegate void TwoUnitDelegate(UnitController dead, UnitController killer);

    public static event VoidDelegate OnRoundEnd;
    public static event VoidDelegate OnRoundStart;
    public static event VoidDelegate OnFirstShowStartupMenu;
    public static event VoidDelegate OnJoinedPlayersReset;
    public static event VoidDelegate OnPlayerJoined;
    public static event VoidDelegate OnEndGame;
    public static event VoidDelegate OnStartGame;
    public static event IntDelegate OnTimerUpdate;
    public static event IntDelegate OnSelectPauseOption;
    public static event IntDelegate OnUpdateScore;
    public static event TwoUnitDelegate OnUnitKilled;
    public static event IntDelegate OnPlayerWon;

    public static ActionsContoller GetActions()
    {
        return GameObject.FindObjectOfType<ActionsContoller>();
    }

    public void RoundRestart()
    {
        EndRound();
        StartCoroutine(StartRoundAfterDelay());
    }

    IEnumerator StartRoundAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        StartRound();
    }

    public void EndRound()
    {
        OnRoundEnd?.Invoke();
    }

    public void UnitKilled(UnitController dead, UnitController killer)
    {
        OnUnitKilled?.Invoke(dead, killer);
    }

    public void StartRound()
    {
        OnRoundStart?.Invoke();
    }

    public void FirstShowStartupMenu()
    {
        OnFirstShowStartupMenu?.Invoke();
    }

    public void ResetJoinedPlayers()
    {
        OnJoinedPlayersReset?.Invoke();
    }

    public void PlayerJoined()
    {
        OnPlayerJoined?.Invoke();
    }
    
    public void EndGame()
    {
        OnEndGame?.Invoke();
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
    }

    public void UpdateTimer(int seconds)
    {
        OnTimerUpdate?.Invoke(seconds);
    }

    public void PlayerWon(int playerId)
    {
        OnPlayerWon?.Invoke(playerId);
    }

    public void SelectPauseOption(int option)
    {
        OnSelectPauseOption?.Invoke(option);
    }

    public void UpdateScore(int playerId)
    {
        OnUpdateScore?.Invoke(playerId);
    }
}
