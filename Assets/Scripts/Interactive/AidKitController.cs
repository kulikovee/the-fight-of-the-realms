using System.Collections;
using UnityEngine;

public class AidKitController : MonoBehaviour
{
    public Animator animator;
    public AudioSource createAidKitSound;
    public AudioSource takeAidKitSound;
    PlayerController[] players;
    bool isDead = true;

    void Start()
    {
        players = GameObject.FindObjectsOfType<PlayerController>();
        StartCoroutine(ShowAfterDelay());
    }

    void Update()
    {
        if (isDead) 
        {
            return;
        }

        foreach (var player in players)
        {
            if (
                !isDead
                && player.GetUnit().IsAlive() 
                && Vector3.Distance(player.transform.position, transform.position + Vector3.up * 0.4f) < 0.6f
            )
            {
                Take(player);
            }
        }
    }

    void Take(PlayerController byPlayer)
    {
        isDead = true;
        takeAidKitSound.Play();
        byPlayer.GetUnit().RestoreHp();
        animator.Play("Die");
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        isDead = false;
        createAidKitSound.Play();
        animator.Play("Show");
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }
}
