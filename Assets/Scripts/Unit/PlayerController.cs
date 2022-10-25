using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    public int score = 0;
    public TextMeshProUGUI playerJoinedText;
    public TextMeshProUGUI playerScoreText;
    public Animator animatorJoin;

    readonly float shakeTimeout = 0.5f;
    float shakeAt = 0;

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
        Unjoin();
        playerJoinedText.SetText("Player " + (playerId + 1) + "\nPress <u>A</u> to join");
    }

    public void UpdateJoinedText()
    {
        if (unit.GetDevice().IsSelected())
        {
            playerJoinedText.SetText("Player " + (playerId + 1) + "\n<b>Joined!</b>");
        }
    }

    public void Join()
    {
        animatorJoin.SetBool("joined", true);
    }

    public void Unjoin()
    {
        animatorJoin.SetBool("joined", false);
    }
    
    public void ShakeJoin()
    {
        if (IsShakeAvailable())
        {
            ResetShakeTimeout();
            animatorJoin.SetTrigger("shake");
        }
    }

    public void ResetShakeTimeout()
    {
        shakeAt = Time.unscaledTime;
    }

    bool IsShakeAvailable()
    {
        return Time.unscaledTime - shakeAt > shakeTimeout;
    }
}
