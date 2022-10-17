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
        ActionsContoller.OnRoundEnd += FreezeAndResetPosition;
        ActionsContoller.OnRoundStart += Unfreeze;
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
        ActionsContoller.OnRoundEnd -= FreezeAndResetPosition;
        ActionsContoller.OnRoundStart -= Unfreeze;
        ActionsContoller.OnUnitKilled -= UpdateScore;
        ActionsContoller.OnStartGame -= ResetScoreToDefault;
    }

    public UnitController GetUnit()
    {
        return unit;
    }

    public void FreezeAndResetPosition()
    {
        unit.SetFrozen(true);
        unit.ResetUnit();
    }

    public void Unfreeze()
    {
        unit.SetFrozen(false);
    }

    public void UpdateScore(UnitController _dead, UnitController killer)
    {
        if (killer == unit)
        {
            actions.UpdateScore(playerId);
        }
    }

    internal void AddScorePoint()
    {
        score++;
        UpdatedScoreText();
    }

    internal void ResetScoreToDefault()
    {
        score = 0;
        UpdatedScoreText();
    }

    public void ResetPlayer()
    {
        unit.GetDevice().ResetDeviceId();
        unit.ResetUnit();
        unit.SetFrozen(true);
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
