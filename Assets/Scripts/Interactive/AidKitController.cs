using System.Collections;
using UnityEngine;

public class AidKitController : MonoBehaviour
{
    public Animator animator;
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
                && Vector3.Distance(player.transform.position - Vector3.up * 0.3f, transform.position) < 0.3f
            )
            {
                Take(player);
            }
        }
    }

    void Take(PlayerController byPlayer)
    {
        isDead = true;
        byPlayer.GetUnit().RestoreHp();
        animator.Play("Die");
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        isDead = false;
        animator.Play("Show");
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
    }
}
