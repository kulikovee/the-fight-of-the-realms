using KinematicCharacterController;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // configurable params
    public int playerId;
    public TextMeshProUGUI playerText;
    private UnitController unit;
    private ActionsContoller actions;

    void Start()
    {
        ActionsContoller.OnResetPlayersText += SetPlayerTextDefault;
        ActionsContoller.OnJoinedPlayersText += JoinedPlayersText;
        ActionsContoller.OnEndGame += ResetPlayer;
        ActionsContoller.OnRoundEnd += FreezeAndResetPosition;
        ActionsContoller.OnRoundStart += Unfreeze;
        ActionsContoller.OnUnitKilled += UpdateScore;

        actions = ActionsContoller.GetActions();
        unit = GetComponent<UnitController>();
    }

    public UnitController GetUnit()
    {
        return unit;
    }

    public void FreezeAndResetPosition()
    {
        unit.Revive();
        unit.SetFrozen(true);
        unit.ResetPosition();
    }

    public void Unfreeze()
    {
        unit.SetFrozen(false);
    }
    public void UpdateScore(UnitController dead, UnitController killer)
    {
        if (killer == unit)
        {
            actions.UpdateScore(
                playerId == 0,
                playerId == 1,
                playerId == 2,
                playerId == 3
            );
        }
    }

    public void ResetPlayer()
    {
        unit.GetDevice().ResetDeviceId();
        unit.ResetPosition();
        unit.SetFrozen(true);
        SetPlayerTextDefault();
    }

    public void SetPlayerTextDefault()
    {
        playerText.SetText("Player " + (playerId + 1) + "\nPress any button");
    }

    public void JoinedPlayersText()
    {
        if (unit.GetDevice().IsSelected())
        {
            playerText.SetText("Player " + (playerId + 1) + "\n<b>Joined!</b>");
        }
    }
}
