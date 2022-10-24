using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : MonoBehaviour
{
    public string arenaSceneName = "arena";
    private Animator animator;

    void Start()
    {
        ActionsController.OnStartGame += Hide;

        animator = GetComponent<Animator>();
        Time.timeScale = 0;

        // Duplicated in LevelController
        Cursor.visible = false;
        Screen.fullScreen = true;
        // Input.simulateMouseWithTouches = true;
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
        SceneManager.LoadScene(arenaSceneName);
    }

    /** Called from animation: Logo Hide **/
    public void Hidden()
    {
        gameObject.SetActive(false);
    }
}
