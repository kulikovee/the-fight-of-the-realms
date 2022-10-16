using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerKilledController : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI text;

    void Start()
    {
        ActionsContoller.OnUnitKilled += ShowKillMessage;
        animator = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
    }

    void OnDestroy()
    {
        ActionsContoller.OnUnitKilled -= ShowKillMessage;
    }

    private void ShowKillMessage(UnitController deadUnit, UnitController killerUnit)
    {
        var deadPlayer = deadUnit.GetComponent<PlayerController>();
        var killerPlayer = killerUnit.GetComponent<PlayerController>();

        Debug.Log("ShowKillMessage : " + (deadPlayer != null) + ", " + (killerPlayer != null));

        if (deadPlayer != null && killerPlayer != null)
        {
            text.text = GetKillMessage(deadPlayer.playerId, killerPlayer.playerId);
            animator.Play("Show");
        }
    }

    private string GetKillMessage(int deadId, int killerId)
    {
        var deadName = $"<color={PlayerWonController.playerColors[deadId]}>P{deadId + 1}</color>";
        var killerName = $"<color={PlayerWonController.playerColors[killerId]}>P{killerId + 1}</color>";

        if (Random.Range(0f, 1f) <= 0.3f)
        {
            // Default message
            return $"{killerName} kills {deadName}";
        } else
        {
            // Enchanted messages
            var messages = new List<string>()
            {
                $"{killerName} gives {deadName} a rest",
                $"{killerName} considers {deadName} pathetic",
                $"{killerName} is having a good time with {deadName}",
                $"{killerName} presents {deadName} a death",
                $"{killerName} lets {deadName} lie down",
                $"{killerName} makes {deadName} sniff the ground",
                $"{killerName} feels sorry for {deadName}",
                $"{killerName} gives a rib sword to {deadName}",
                $"{killerName} gives bleeding to {deadName}",
                $"{killerName} helps {deadName} to look at the sky",
                $"{killerName} makes {deadName} needs the first aid",
                $"{killerName} shows {deadName} the last light",
                $"{killerName} makes {deadName} to cry",
                $"{killerName} makes {deadName} to pray",
                $"{killerName} makes {deadName} kiss his boots",
                $"{killerName} laughs at {deadName}",
                $"{killerName} is {deadName}'s boss",
                $"{killerName} destroyes {deadName}",
                $"{killerName} annihilates {deadName}",
                $"{killerName} demoralizes {deadName}",
                $"{killerName} upsets {deadName}",
                $"{killerName} knockdowns {deadName}",
                $"{killerName} knocks out {deadName}",
                $"{killerName} devastates {deadName}",
                $"{killerName} kicks out {deadName}",
                $"{killerName} presses {deadName}",
                $"{killerName} removes {deadName}",
                $"{killerName} treats {deadName}",
                $"{killerName} kicks {deadName}",
                $"{killerName} shocks {deadName}",
                $"{killerName} strikes {deadName}",
                $"{killerName} owns {deadName}",
                $"{killerName} beats {deadName}",
                $"{killerName} frees {deadName}",
                $"{killerName} punishes {deadName}",
                $"{killerName} liberates {deadName}",
                $"{killerName} crunches {deadName}"
            };

            return messages[Random.Range(0, messages.Count)];
        }
    }
}
