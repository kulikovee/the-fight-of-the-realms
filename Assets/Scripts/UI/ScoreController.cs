using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public List<AudioSource> scoreUpdateSounds;
    public TextMeshProUGUI scoreText0;
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    public TextMeshProUGUI scoreText3;
    public AudioSource gameOverSound;

    private Animator animator;
    private ActionsContoller actions;
    private bool isShown = false;

    private int winScore = 10;
    private int playerScore0 = 0;
    private int playerScore1 = 0;
    private int playerScore2 = 0;
    private int playerScore3 = 0;

    private void Start()
    {
        ActionsContoller.OnStartGame += Show;
        ActionsContoller.OnUpdateScore += UpdateScore;
        ActionsContoller.OnStartGame += ResetScore;

        actions = ActionsContoller.GetActions();
        animator = GetComponent<Animator>();
    }

    /** Called from animation: Score Update **/
    public void PlayScoreUpdateSound()
    {
        scoreUpdateSounds[Random.Range(0, scoreUpdateSounds.Count)].Play();
    }

    public void Show()
    {
        if (!isShown)
        {
            isShown = true;
            animator.Play("Score Show");
        }
    }

    public void UpdateScore(bool isFirstPlayer, bool isSecondPlayer, bool isThirdPlayer, bool isForthPlayer)
    {
        if (isFirstPlayer)
        {
            playerScore0++;
        }

        if (isSecondPlayer)
        {
            playerScore1++;
        }

        if (isThirdPlayer)
        {
            playerScore2++;
        }

        if (isForthPlayer)
        {
            playerScore3++;
        }

        UpdateScoreText();

        if (
            isFirstPlayer
            || isSecondPlayer
            || isThirdPlayer
            || isForthPlayer
        )
        {
            animator.Play("Score Update");
        }

        if (playerScore0 == winScore || playerScore1 == winScore || playerScore2 == winScore || playerScore3 == winScore)
        {
            gameOverSound.Play();
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(0.5f);
        actions.EndGame();
    }

    public void ResetScore()
    {
        playerScore0 = 0;
        playerScore1 = 0;
        playerScore2 = 0;
        playerScore3 = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText0.text = "P1: " + playerScore0;
        scoreText1.text = "P2: " + playerScore1;
        scoreText2.text = "P3: " + playerScore2;
        scoreText3.text = "P4: " + playerScore3;
    }
}
