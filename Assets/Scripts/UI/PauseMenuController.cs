using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // sounds
    public AudioSource toggleSound;
    public AudioSource showSound;
    public AudioSource hideSound;

    // consts
    private const float togglePauseTimeout = 0.25f;
    private const float toggleValueTimeout = 0.2f;
    private const float submitValueTimeout = 0.2f;

    // params
    private bool isVisible = false;
    private bool isDisabled = true;

    // links
    private ActionsController actions;
    private Animator animator;

    // togglers
    private int optionsCount = 0;
    private int selectedOption = 0;
    private float lastPauseAt = 0;
    private float lastValueAt = 0;
    private float lastSubmitAt = 0;

    void Start()
    {
        ActionsController.OnEndGame += Disable;
        ActionsController.OnStartGame += Enable;

        actions = ActionsController.GetActions();
        animator = GetComponent<Animator>();
        optionsCount = GameObject.FindObjectsOfType<PauseMenuItemController>().Length;
    }
    
    void OnDestroy()
    {
        ActionsController.OnEndGame -= Disable;
        ActionsController.OnStartGame -= Enable;
    }

    void Update()
    {
        if (!isDisabled)
        {
            Navigate();
        }
    }

    private void Disable()
    {
        isDisabled = true;
    }

    private void Enable()
    {
        isDisabled = false;
        UpdatePauseTimeout();
    }

    void UpdatePauseTimeout()
    {
        lastPauseAt = Time.unscaledTime;
    }

    void ToggleVisibility()
    {
        if (Time.unscaledTime - lastPauseAt >= togglePauseTimeout)
        {
            SetVisibility(!isVisible);
            UpdatePauseTimeout();
        }
    }

    void Navigate()
    {
        var axis = InputGlobalController.GetUpdatedAxis();

        if (axis.GetPause() != 0)
        {
            ToggleVisibility();
        }

        if (isVisible)
        {
            if (axis.GetY() != 0)
            {
                SelectOption((int) axis.GetY());
            }

            if (axis.GetButtonA() != 0)
            {
                Submit();
            }
        }
    }
    
    void SetVisibility(bool newVisibility)
    {
        isVisible = newVisibility;

        if (isVisible)
        {
            Time.timeScale = 0;
            animator.Play("Pause Menu Show");
            showSound.Play();
            actions.SelectPauseOption(selectedOption);
        }
        else
        {
            Time.timeScale = 1;
            animator.Play("Pause Menu Hide");
            hideSound.Play();
        }
    }

    void SelectOption(int increment)
    {
        if (
            Time.unscaledTime - lastValueAt >= toggleValueTimeout 
            && selectedOption + increment >= 0 
            && selectedOption + increment < optionsCount
        )
        {
            lastValueAt = Time.unscaledTime;
            selectedOption += increment;
            actions.SelectPauseOption(selectedOption);
            toggleSound.Play();
        }
    }

    void Submit()
    {
        if (Time.unscaledTime - lastSubmitAt >= submitValueTimeout)
        {
            lastSubmitAt = Time.unscaledTime;

            switch (selectedOption)
            {
                case 0:
                    SetVisibility(false);
                    break;
                case 1:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
                case 2:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }
    }
}
