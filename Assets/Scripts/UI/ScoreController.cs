using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreController : MonoBehaviour
{
    // Global Configuration
    public static int killsToWin = 15;

    // Configurable
    public List<AudioSource> scoreUpdateSounds;
    public AudioSource gameOverSound;

    // Params
    bool isShown = false;
    bool isRestarting = false;
    Animator animator;
    ActionsContoller actions;
    LevelController level;

    // Players
    public List<PlayerController> players = new() { };
    public List<UnitController> enemies = new() { };

    void Start()
    {
        ActionsContoller.OnStartGame += ShowScorePanel;
        ActionsContoller.OnUpdateScore += UpdateScore;
        ActionsContoller.OnUnitKilled += CheckWinner;

        actions = ActionsContoller.GetActions();
        level = LevelController.GetLevel();
        animator = GetComponent<Animator>();

        foreach (var unit in GameObject.FindObjectsOfType<UnitController>())
        {
            if (unit.team == "enemy")
            {
                enemies.Add(unit);
            }
            else
            {
                var player = unit.GetComponent<PlayerController>();
                if (player != null)
                {
                    players.Add(player);
                }
            }
        }
    }

    void OnDestroy()
    {
        ActionsContoller.OnStartGame -= ShowScorePanel;
        ActionsContoller.OnUpdateScore -= UpdateScore;
        ActionsContoller.OnUnitKilled -= CheckWinner;
    }

    /** Called from animation: Score Update **/
    public void PlayScoreUpdateSound()
    {
        scoreUpdateSounds[Random.Range(0, scoreUpdateSounds.Count)].Play();
    }

    void CheckWinner(UnitController _dead, UnitController killer)
    {
        if (killer.team == "enemy")
        {
            // Restart if no winner and players dead
            RestartRoundIfLevelCompleted();
        }
    }

    void UpdateScore(int scoreUpdatePlayerId, int scoreUpdate)
    {
        animator.Play("Score Update");

        // Check previous winner
        if (GetWinnerPlayer() != null)
        {
            return;
        }

        // Check current winner
        var winnerPlayer = GetWinnerPlayer();

        if (winnerPlayer != null)
        {
            actions.EndRound();
            actions.PlayerWon(winnerPlayer.playerId);
            gameOverSound.Play();
        } else
        {
            // Restart if no winner and players dead
            RestartRoundIfLevelCompleted();
        }
    }

    void ShowScorePanel()
    {
        if (!isShown)
        {
            isShown = true;
            animator.Play("Score Show");
        }
    }

    PlayerController GetWinnerPlayer()
    {
        foreach(var player in players)
        {
            if (player.score >= killsToWin)
            {
                return player;
            }
        }

        return null;
    }

    void RestartRoundIfLevelCompleted()
    {
        if (isRestarting)
        {
            return;
        }

        var alivePlayersCount = 0;
        var aliveControlledPlayersCount = 0;
        var aliveEnemiesCount = 0;
        var alivePlayersToRestart = level.IsDeathmatch() ? 1 : 0;

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
        
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                aliveEnemiesCount++;
            }
        }


        if (alivePlayersCount <= alivePlayersToRestart || aliveControlledPlayersCount == 0 || (level.IsBoss() && aliveEnemiesCount == 0))
        {
            isRestarting = true;
            StartCoroutine(RestartAfterDelay());
        }
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        isRestarting = false;
        actions.RoundRestart();
    }
}
