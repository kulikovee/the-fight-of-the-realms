using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseResumeController : MonoBehaviour
{
    public const int optionId = 0;

    private TextMeshProUGUI text;
    private Image panel;
    private Outline panelOutline;

    void Start()
    {
        ActionsContoller.OnSelectPauseOption += UpdateSelection;

        text = GetComponent<TextMeshProUGUI>();
        panel = gameObject.transform.parent.GetComponent<Image>();
        panelOutline = gameObject.transform.parent.GetComponent<Outline>();
    }

    void UpdateSelection(int option)
    {
        text.SetText(option == optionId ? "<b>Resume</b>" : "Resume");
        panel.color = new Color(0.7f, 0.05f, 0.05f, option == optionId ? 1 : 0.7f);
        panelOutline.effectColor = new Color(1, 1, 0, option == optionId ? 1 : 0);
    }
}
