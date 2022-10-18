using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidKitController : MonoBehaviour
{
    public Animator animator;
    public AudioSource createAidKitSound;
    public AudioSource takeAidKitSound;
    public List<Vector3> respawnPoints;
    public bool isDead = true;

    UnitController[] units;

    void Start()
    {
        units = GameObject.FindObjectsOfType<UnitController>();
        StartCoroutine(ShowAfterDelay());
    }

    void Update()
    {
        if (isDead) 
        {
            return;
        }

        foreach (var unit in units)
        {
            if (
                !isDead
                && unit.IsAlive()
                && unit.canTakeItems
                && Vector3.Distance(unit.transform.position, transform.position + Vector3.up * 0.4f) < 0.6f
            )
            {
                Take(unit);
            }
        }
    }

    void Take(UnitController byUnit)
    {
        isDead = true;
        takeAidKitSound.Play();
        byUnit.AddHp(50);
        animator.Play("Die");
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        isDead = false;
        createAidKitSound.Play();
        animator.Play("Show");
        transform.position = respawnPoints[Random.Range(0, respawnPoints.Count)];
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }
}
