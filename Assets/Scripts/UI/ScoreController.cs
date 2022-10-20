using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreController : MonoBehaviour
{
    // Configurable
    public List<AudioSource> scoreUpdateSounds;

    // Params
    bool isShown = false;
    Animator animator;

    void Start()
    {
        ActionsController.OnStartGame += ShowScorePanel;
        ActionsController.OnScoreUpdate += OnUpdateScore;

        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        ActionsController.OnStartGame -= ShowScorePanel;
        ActionsController.OnScoreUpdate -= OnUpdateScore;
    }

    /** Called from animation: Score Update **/
    public void PlayScoreUpdateSound()
    {
        scoreUpdateSounds[Random.Range(0, scoreUpdateSounds.Count)].Play();
    }

    void OnUpdateScore()
    {
        animator.Play("Score Update");
    }

    void ShowScorePanel()
    {
        if (!isShown)
        {
            isShown = true;
            animator.Play("Score Show");
        }
    }
}
