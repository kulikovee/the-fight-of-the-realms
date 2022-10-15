using TMPro;
using UnityEngine;

public class PauseExitController : MonoBehaviour
{
    public const int optionId = 1;

    private TextMeshProUGUI text;

    void Start()
    {
        ActionsContoller.OnSelectPauseOption += UpdateSelection;

        text = GetComponent<TextMeshProUGUI>();
    }

    void UpdateSelection(int option)
    {
        text.SetText(option == optionId ? "<b>Exit Game</b>" : "Exit Game");
        text.fontStyle = option == optionId ? FontStyles.Underline : FontStyles.Normal;
    }
}
