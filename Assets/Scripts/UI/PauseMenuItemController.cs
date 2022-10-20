using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuItemController : MonoBehaviour
{
    public int optionId = -1;

    private TextMeshProUGUI text;
    private Image panel;
    private Outline panelOutline;
    private string buttonText;
    private Color buttonColor;
    private Color outlineColor;

    void Start()
    {
        if (optionId == -1)
        {
            Debug.LogError("PauseMenuItemController optionId is not set!");
        }

        ActionsController.OnSelectPauseOption += UpdateSelection;

        panel = GetComponent<Image>();
        panelOutline = GetComponent<Outline>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        buttonText = text.text;
        buttonColor = panel.color;
        outlineColor = panelOutline.effectColor;
    }

    void OnDestroy()
    {
        ActionsController.OnSelectPauseOption -= UpdateSelection;
    }

    void UpdateSelection(int option)
    {
        text.fontStyle = option == optionId ? FontStyles.Bold : FontStyles.Normal;
        panel.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, option == optionId ? 1 : 0.7f);
        panelOutline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, option == optionId ? 1 : 0);
    }
}
