using TMPro;
using UnityEngine;

public class TimerTextController : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        ActionsContoller.OnTimerUpdate += UpdateTimer;
        text = GetComponent<TextMeshProUGUI>();
    }

    void OnDestroy()
    {
        ActionsContoller.OnTimerUpdate -= UpdateTimer;
    }

    void UpdateTimer(int seconds)
    {
        text.SetText("Start in " + seconds + "...");

        if (seconds <= 0)
        {
            text.SetText("Waiting a player joined...");
        }
    }
}
