using TMPro;
using UnityEngine;

public class PauseResumeController : MonoBehaviour
{
    public const int optionId = 0;

    private TextMeshProUGUI text;

    void Start()
    {
        ActionsContoller.OnSelectPauseOption += UpdateSelection;

        text = GetComponent<TextMeshProUGUI>();
    }

    void UpdateSelection(int option)
    {
        text.SetText(option == optionId ? "<b>Resume</b>" : "Resume");
        text.fontStyle = option == optionId ? FontStyles.Underline : FontStyles.Normal;
    }
}
