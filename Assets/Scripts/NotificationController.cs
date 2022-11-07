using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    public static string npcColor = "#ccc";
    public static List<string> playerColors = new() { "#faa", "#afa", "#aaf", "#fff" };

    public GameObject notificationPrefab;
    public GameObject container;
    PlayerController[] players;

    void Start()
    {
        players = GameObject.FindObjectsOfType<PlayerController>();
    }

    public void Notify(UnitController unit, string text)
    {
        if (unit != null && unit.hpBarImage != null)
        {
            var notification = Instantiate(notificationPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
            var textMesh = notification.GetComponent<TextMeshProUGUI>();
            var color = GetUnitColor(unit);
            textMesh.transform.SetParent(unit.hpBarImage.transform.parent, false);
            textMesh.rectTransform.anchoredPosition = new Vector3(55, -155, 0);
            textMesh.text = $"<color={color}>{text}</color>";
        }
    }

    public static NotificationController GetNotifications()
    {
        return GameObject.FindObjectOfType<NotificationController>();
    }

    string GetUnitColor(UnitController unit)
    {
        foreach (var player in players)
        {
            if (player.GetUnit() == unit)
            {
                return playerColors[player.playerId];
            }
        }

        var npcColorId = playerColors.Count - 1;
        return playerColors[npcColorId];
    }
}
