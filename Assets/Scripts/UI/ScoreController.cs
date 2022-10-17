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
    Animator animator;
    ActionsContoller actions;

    // Players
    public PlayerController[] playerControllers;

    void Start()
    {
        ActionsContoller.OnStartGame += ShowScorePanel;
        ActionsContoller.OnUpdateScore += UpdateScore;

        playerControllers = GameObject.FindObjectsOfType<PlayerController>();
        actions = ActionsContoller.GetActions();
        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        ActionsContoller.OnStartGame -= ShowScorePanel;
        ActionsContoller.OnUpdateScore -= UpdateScore;
    }
    /** Called from animation: Score Update **/
    public void PlayScoreUpdateSound()
    {
        scoreUpdateSounds[Random.Range(0, scoreUpdateSounds.Count)].Play();
    }

    void UpdateScore(int scoreUpdatePlayerId)
    {
        // Check previous winner
        if (GetWinnerPlayer() != null)
        {
            return;
        }

        FindPlayerById(scoreUpdatePlayerId).AddScorePoint();
        animator.Play("Score Update");

        // Check current winner
        var winnerPlayer = GetWinnerPlayer();

        if (winnerPlayer != null)
        {
            actions.EndRound();
            actions.PlayerWon(winnerPlayer.playerId);
            gameOverSound.Play();
        }
        else
        {
            // Restart if no winner and players dead
            RestartRoundIfPlayersDead();
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

    PlayerController FindPlayerById(int playerId)
    {
        foreach (var player in playerControllers)
        {
            if (player.playerId == playerId)
            {
                return player;
            }
        }

        return null;
    }

    PlayerController GetWinnerPlayer()
    {
        foreach(var player in playerControllers)
        {
            if (player.score >= killsToWin)
            {
                return player;
            }
        }

        return null;
    }

    void RestartRoundIfPlayersDead()
    {
        var alivePlayersCount = 0;
        var aliveControlledPlayersCount = 0;

        foreach (var player in playerControllers)
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
            StartCoroutine(RestartAfterDelay());
        }
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        actions.RoundRestart();
    }
}
