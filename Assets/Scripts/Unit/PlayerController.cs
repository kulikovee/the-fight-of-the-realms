using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    public int score = 0;
    public TextMeshProUGUI playerJoinedText;
    public TextMeshProUGUI playerScoreText;

    UnitController unit;
    ActionsController actions;

    void Start()
    {
        ActionsController.OnJoinedPlayersReset += ResetJoinedTextToDefault;
        ActionsController.OnPlayerJoined += UpdateJoinedText;
        ActionsController.OnEndGame += ResetPlayer;
        ActionsController.OnStartGame += ResetScoreToDefault;

        actions = ActionsController.GetActions();
        unit = GetComponent<UnitController>();
    }

    void OnDestroy()
    {
        ActionsController.OnJoinedPlayersReset -= ResetJoinedTextToDefault;
        ActionsController.OnPlayerJoined -= UpdateJoinedText;
        ActionsController.OnEndGame -= ResetPlayer;
        ActionsController.OnStartGame -= ResetScoreToDefault;
    }

    public UnitController GetUnit()
    {
        return unit;
    }

    public void AddScore(int scorePoints)
    {
        score += scorePoints;
        UpdatedScoreText();
        actions.UpdateScore();
    }

    internal void ResetScoreToDefault()
    {
        score = 0;
        UpdatedScoreText();
    }

    public void ResetPlayer()
    {
        ResetJoinedTextToDefault();
    }

    public void UpdatedScoreText()
    {
        playerScoreText.text = $"P{playerId + 1}: {score}";
    }

    public void ResetJoinedTextToDefault()
    {
        playerJoinedText.SetText("Player " + (playerId + 1) + "\nPress any button");
    }

    public void UpdateJoinedText()
    {
        if (unit.GetDevice().IsSelected())
        {
            playerJoinedText.SetText("Player " + (playerId + 1) + "\n<b>Joined!</b>");
        }
    }
}
