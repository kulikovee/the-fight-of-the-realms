using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerKilledController : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI text;

    void Start()
    {
        ActionsController.OnUnitKilled += ShowKillMessage;
        animator = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
    }

    void OnDestroy()
    {
        ActionsController.OnUnitKilled -= ShowKillMessage;
    }

    private void ShowKillMessage(UnitController deadUnit, UnitController killerUnit)
    {
        var deadPlayer = deadUnit.GetComponent<PlayerController>();
        var killerPlayer = killerUnit.GetComponent<PlayerController>();

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

        if (deadId == killerId)
        {
            return $"{killerName} gracefully kills himself";
        }

        if (Random.Range(0f, 1f) <= 0.25f)
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
                $"{killerName} gives a rib-sword to {deadName}",
                $"{killerName} gives bleeding to {deadName}",
                $"{killerName} helps {deadName} to look at the sky",
                $"{killerName} makes {deadName} to need first aid",
                $"{killerName} shows {deadName} the last light",
                $"{killerName} makes {deadName} cry",
                $"{killerName} makes {deadName} pray",
                $"{killerName} treats {deadName} with a sword",
                $"{killerName} allow {deadName} to kiss his boots",
                $"{killerName} laughs at {deadName}",
                $"{killerName} is a boss of {deadName}",
                $"{killerName} destroyes {deadName}",
                $"{killerName} annihilates {deadName}",
                $"{killerName} demoralizes {deadName}",
                $"{killerName} upsets {deadName}",
                $"{killerName} knockdowns {deadName}",
                $"{killerName} knocks out {deadName}",
                $"{killerName} devastates {deadName}",
                $"{killerName} kicks out {deadName}",
                $"{killerName} presses {deadName}",
                $"{killerName} threatens {deadName}",
                $"{killerName} kicks {deadName}",
                $"{killerName} shocks {deadName}",
                $"{killerName} strikes {deadName}",
                $"{killerName} owns {deadName}",
                $"{killerName} beats {deadName}",
                $"{killerName} frees {deadName}",
                $"{killerName} punishes {deadName}",
                $"{killerName} liberates {deadName}",
                $"{killerName} smashes {deadName}"
            };

            return messages[Random.Range(0, messages.Count)];
        }
    }
}
