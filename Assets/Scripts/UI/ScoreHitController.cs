using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHitController : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = $"Finish <u>{ScoreController.killsToWin}</b></u> Players to win the tournament";
    }
}
