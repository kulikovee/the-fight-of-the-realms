using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWonController : MonoBehaviour
{
    public static List<string> playerColors = new() { 
        "#f66",
        "#6f6", 
        "#33f",
        "#fff",
        "#ccc" // <-- NPC
    };

    public AudioSource wonSound;
    Animator animator;
    TextMeshProUGUI text;
    ActionsController actions;

    void Start()
    {
        ActionsController.OnPlayerWon += Show;
        animator = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
        actions = ActionsController.GetActions();
    }

    void OnDestroy()
    {
        ActionsController.OnPlayerWon -= Show;
    }

    public void Show(PlayerController player)
    {
        var color = playerColors[player.playerId];
        var playerNumber = player.playerId + 1;
        text.text = $"<color={color}>Player {playerNumber}</color> won the tournament!<br>His score is <u>{player.score}</u>!";
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
