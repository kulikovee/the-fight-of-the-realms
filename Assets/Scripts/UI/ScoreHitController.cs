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
        text.text = $"Reach <u><b>{ScoreController.killsToWin}</b></u> score points to win the tournament";
    }
}
