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
    ActionsContoller actions;

    void Start()
    {
        ActionsContoller.OnJoinedPlayersReset += ResetJoinedTextToDefault;
        ActionsContoller.OnPlayerJoined += UpdateJoinedText;
        ActionsContoller.OnEndGame += ResetPlayer;
        ActionsContoller.OnUnitKilled += UpdateScore;
        ActionsContoller.OnStartGame += ResetScoreToDefault;

        actions = ActionsContoller.GetActions();
        unit = GetComponent<UnitController>();
    }

    void OnDestroy()
    {
        ActionsContoller.OnJoinedPlayersReset -= ResetJoinedTextToDefault;
        ActionsContoller.OnPlayerJoined -= UpdateJoinedText;
        ActionsContoller.OnEndGame -= ResetPlayer;
        ActionsContoller.OnUnitKilled -= UpdateScore;
        ActionsContoller.OnStartGame -= ResetScoreToDefault;
    }

    public UnitController GetUnit()
    {
        return unit;
    }

    public void UpdateScore(UnitController dead, UnitController killer)
    {
        if (killer == unit)
        {
            var scorePoints = dead.team == "enemy" ? 3 : 1;
            actions.UpdateScore(playerId, scorePoints);
            AddScorePoint(scorePoints);
        }
    }

    internal void AddScorePoint(int scoreUpdate)
    {
        score += scoreUpdate;
        UpdatedScoreText();
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
