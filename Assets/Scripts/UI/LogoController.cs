using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : MonoBehaviour
{
    public string arenaSceneName = "arena";
    public AudioSource hitSound1;
    public AudioSource hitSound2;
    public AudioSource waveSound1;
    public AudioSource waveSound2;
    public AudioSource showTitleSound;

    void Start()
    {
        Time.timeScale = 0.75f;
        StartCoroutine(SetTimeScaleAfterDelay(2.95f, 0.02f));
        StartCoroutine(SetTimeScaleAfterDelay(2.97f, 0.01f));
        StartCoroutine(SetTimeScaleAfterDelay(2.99f, 0.005f));
    }

    IEnumerator SetTimeScaleAfterDelay(float delay, float timeScale)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = timeScale;
    }

    /** Called from animation: Show **/
    public void ShowStartupMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(arenaSceneName);
    }

    /** Called from animation: Show **/
    public void PlayHitSound1()
    {
        hitSound1.Play();
    }
    /** Called from animation: Show **/
    public void PlayHitSound2()
    {
        hitSound1.Play();
    }

    /** Called from animation: Show **/
    public void PlayWaveSound1()
    {
        waveSound1.Play();
    }

    /** Called from animation: Show **/
    public void PlayWaveSound2()
    {
        waveSound2.Play();
    }


    /** Called from animation: Show **/
    public void PlayShowTitleSound()
    {
        showTitleSound.Play();
    }
}
