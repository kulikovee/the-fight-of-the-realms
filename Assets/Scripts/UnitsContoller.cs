using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsContoller : MonoBehaviour
{
    public List<PlayerController> players;

    private ActionsContoller actions;

    void Start()
    {
        ActionsContoller.OnUnitKilled += RestartRound;
        actions = ActionsContoller.GetActions();
    }
    void RestartRound(UnitController dead, UnitController killer)
    {
        var alivePlayersCount = 0;
        var aliveControlledPlayersCount = 0;

        foreach (var player in players)
        {
            if (player.GetUnit().IsAlive())
            {
                alivePlayersCount++;

                if (player.GetUnit().GetDevice().IsSelected())
                {
                    aliveControlledPlayersCount++;
                }
            }
        }

        if (alivePlayersCount <= 1 || aliveControlledPlayersCount == 0)
        {
            StartCoroutine(RestartRoundAfterDelay());
        }
    }

    IEnumerator RestartRoundAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        actions.RoundRestart();
    }

}
