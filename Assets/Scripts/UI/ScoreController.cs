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
            // Restart if boss killed all players
            RestartIfRoundEnded();
        }
    }

    void UpdateScore(PlayerController player, int scorePoints)
    {
        // Check previous winner
        if (GetWinnerPlayer() != null)
        {
            return;
        }

        animator.Play("Score Update");
        player.AddScorePoint(scorePoints);

        // Check current winner
        var winnerPlayer = GetWinnerPlayer();

        if (winnerPlayer != null)
        {
            actions.EndRound();
            actions.PlayerWon(winnerPlayer);
            gameOverSound.Play();
        } else
        {
            // Restart if no winner and players dead
            RestartIfRoundEnded();
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

    void RestartIfRoundEnded()
    {
        if (isRestarting)
        {
            return;
        }

        if (IsRoundEnded())
        {
            isRestarting = true;
            StartCoroutine(RestartAfterDelay());
        }
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        actions.RestartRound();
        isRestarting = false;
    }

    bool IsRoundEnded()
    {
        if (level.IsRabbitsCollection())
        {
            // Check if Rabbits level completed
            if (level.rabbitsCollected >= level.rabbitsCollectToWin)
            {
                return true;
            }
        } else
        {
            // Check if all players dead
            var alivePlayersCount = 0;
            var aliveControlledPlayersCount = 0;
            // If boss level, wait for all players dead
            var alivePlayersToRestart = level.IsBoss() ? 0 : 1;

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

            if (alivePlayersCount <= alivePlayersToRestart || aliveControlledPlayersCount == 0)
            {
                return true;
            }
        }

        if (level.IsBoss())
        {
            // Check if Boss dead
            var aliveEnemiesCount = 0;

            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive())
                {
                    aliveEnemiesCount++;
                }
            }

            if (aliveEnemiesCount == 0)
            {
                return true;
            }
        }

        return false;
    }
}
