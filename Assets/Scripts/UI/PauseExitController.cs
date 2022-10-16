using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseExitController : MonoBehaviour
{
    public const int optionId = 1;

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
        text.SetText(option == optionId ? "<b>Exit Game</b>" : "Exit Game");
        panel.color = new Color(0.05f, 0.05f, 0.7f, option == optionId ? 1 : 0.7f);
        panelOutline.effectColor = new Color(1, 1, 0, option == optionId ? 1 : 0);
    }
}
