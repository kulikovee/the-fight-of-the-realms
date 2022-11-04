using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuItemController : MonoBehaviour
{
    public int optionId = -1;

    private TextMeshProUGUI text;
    private Image panel;
    private Color buttonColor;

    void Start()
    {
        if (optionId == -1)
        {
            Debug.LogError("PauseMenuItemController optionId is not set!");
        }

        ActionsController.OnSelectPauseOption += UpdateSelection;

        panel = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        buttonColor = panel.color;
    }

    void OnDestroy()
    {
        ActionsController.OnSelectPauseOption -= UpdateSelection;
    }

    void UpdateSelection(int option)
    {
        text.fontStyle = option == optionId ? FontStyles.Bold : FontStyles.Normal;
        panel.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, option == optionId ? 0.1f : 0);
    }
}
