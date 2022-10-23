using System;
using System.Collections;
using UnityEngine;

public class ActionsController : MonoBehaviour
{
    public delegate void VoidDelegate();
    public delegate void IntDelegate(int value);
    public delegate void TwoIntDelegate(int value1, int value2);
    public delegate void PlayerDelegate(PlayerController player);
    public delegate void PlayerIntDelegate(PlayerController player, int value);
    public delegate void TwoUnitDelegate(UnitController dead, UnitController killer);
    public delegate void UnitItemDelegate(UnitController unit, ItemController item);
    public delegate void UnitDelegate(UnitController unit);

    public static event VoidDelegate OnRoundEnd;
    public static event VoidDelegate OnRoundStart;
    public static event VoidDelegate OnRoundRestart;
    public static event VoidDelegate OnFirstShowStartupMenu;
    public static event VoidDelegate OnJoinedPlayersReset;
    public static event VoidDelegate OnPlayerJoined;
    public static event VoidDelegate OnEndGame;
    public static event VoidDelegate OnStartGame;
    public static event IntDelegate OnTimerUpdate;
    public static event IntDelegate OnSelectPauseOption;
    public static event VoidDelegate OnScoreUpdate;
    public static event TwoUnitDelegate OnUnitKilled;
    public static event PlayerDelegate OnPlayerWon;
    public static event UnitItemDelegate OnItemPickUp;

    static bool isRoundRestarting = false;

    public static ActionsController GetActions()
    {
        return GameObject.FindObjectOfType<ActionsController>();
    }

    public void RestartRound()
    {
        if (isRoundRestarting)
        {
            return;
        }

        isRoundRestarting = true;
        OnRoundRestart?.Invoke();
        EndRound();
        StartCoroutine(StartRoundAfterDelay());
    }

    IEnumerator StartRoundAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        StartRound();
        isRoundRestarting = false;
    }

    public void EndRound()
    {
        OnRoundEnd?.Invoke();
    }

    public void UnitKilled(UnitController dead, UnitController killer)
    {
        OnUnitKilled?.Invoke(dead, killer);
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

    public void PickUpItem(UnitController unit, ItemController item)
    {
        OnItemPickUp?.Invoke(unit, item);
    }

    public void UpdateTimer(int seconds)
    {
        OnTimerUpdate?.Invoke(seconds);
    }

    public void PlayerWon(PlayerController player)
    {
        OnPlayerWon?.Invoke(player);
    }

    public void SelectPauseOption(int option)
    {
        OnSelectPauseOption?.Invoke(option);
    }

    public void UpdateScore()
    {
        OnScoreUpdate?.Invoke();
    }
    
    private void StartRound()
    {
        OnRoundStart?.Invoke();
    }
}
