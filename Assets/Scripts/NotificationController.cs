using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    public GameObject notificationPrefab;
    public GameObject container;
    Camera mainCamera;
    PlayerController[] players;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        players = GameObject.FindObjectsOfType<PlayerController>();
    }

    public void Notify(UnitController unit, string text)
    {
        var screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position + Vector3.up);
        if (screenPosition.z > 0)
        {
            var notification = Instantiate(notificationPrefab, Vector3.zero, Quaternion.identity);
            var textMesh = notification.GetComponent<TextMeshProUGUI>();
            var color = GetUnitColor(unit);
            textMesh.transform.SetParent(container.transform, false);
            textMesh.rectTransform.anchoredPosition = new Vector3(screenPosition.x, screenPosition.y, 0);
            textMesh.text = $"<color={color}>{text}</color>";
        }
    }

    public static NotificationController GetNotifications()
    {
        return GameObject.FindObjectOfType<NotificationController>();
    }

    string GetUnitColor(UnitController unit)
    {
        var playerColors = PlayerWonController.playerColors;

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
