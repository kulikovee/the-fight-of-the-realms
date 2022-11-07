using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : MonoBehaviour
{
    public string arenaSceneName = "arena";
    public AudioSource hitSound1;
    public AudioSource waveSound1;
    public AudioSource waveSound2;
    public AudioSource showTitleSound;

    /** Called from animation: Show **/
    public void ShowStartupMenu()
    {
        SceneManager.LoadScene(arenaSceneName);
    }

    /** Called from animation: Show **/
    public void PlayHitSound1()
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
