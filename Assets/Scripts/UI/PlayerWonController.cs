using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWonController : MonoBehaviour
{
    public static List<string> playerColors = new() { "#f66", "#6f6", "#33f", "#f3f" };

    public AudioSource wonSound;
    Animator animator;
    TextMeshProUGUI text;
    ActionsContoller actions;

    void Start()
    {
        ActionsContoller.OnPlayerWon += Show;
        animator = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
        actions = ActionsContoller.GetActions();
    }

    void OnDestroy()
    {
        ActionsContoller.OnPlayerWon -= Show;
    }

    public void Show(int playerId)
    {
        var color = playerColors[playerId];
        var playerNumber = playerId + 1;
        text.text = $"<color={color}>Player {playerNumber}</color> won the tournament!<br>He finished <u>{ScoreController.killsToWin}</u> Players!";
        animator.Play("Show");
    }


    /** Called from animation: Show **/
    public void PlayWonSound()
    {
        wonSound.Play();
    }

    /** Called from animation: Show **/
    public void EndGame()
    {
        actions.EndGame();
    }
}
