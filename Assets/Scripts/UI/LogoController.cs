using UnityEngine;

public class LogoController : MonoBehaviour
{
    private ActionsController actions;
    private Animator animator;

    void Start()
    {
        ActionsController.OnStartGame += Hide;

        actions = ActionsController.GetActions();
        animator = GetComponent<Animator>();
        Time.timeScale = 0;
    }

    void OnDestroy()
    {
        ActionsController.OnStartGame -= Hide;
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            animator.Play("Logo Hide");
        }
    }

    /** Called from animation: Logo Idle **/
    public void ShowStartupMenu()
    {
        actions.FirstShowStartupMenu();
    }

    /** Called from animation: Logo Hide **/
    public void Hidden()
    {
        gameObject.SetActive(false);
    }
}
