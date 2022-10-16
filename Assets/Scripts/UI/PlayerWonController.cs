using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWonController : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI text;
    private ActionsContoller actions;
    private List<string> playerColors = new() { "#f66", "#6f6", "#33f", "#f3f" };

    void Start()
    {
        ActionsContoller.OnPlayerWon += Show;
        animator = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
        actions = ActionsContoller.GetActions();
    }

    public void Show(int playerId)
    {
        var color = playerColors[playerId];
        var playerNumber = playerId + 1;
        text.text = $"<color={color}>Player {playerNumber}</color> won the tournament!";
        animator.Play("Show");
    }

    /** Called from animation: Show **/
    public void Hidden()
    {
        actions.EndGame();
    }
}
