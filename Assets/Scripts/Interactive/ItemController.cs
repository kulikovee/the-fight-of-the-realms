using System.Collections;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Animator animator;
    public AudioSource createAidKitSound;
    public AudioSource takeAidKitSound;

    float pickUpTimeout = 0.3f;
    float createdAt = 0;
    bool isDead = false;
    ActionsController actions;

    void Start()
    {
        actions = ActionsController.GetActions();
        animator.Play("Show");
        createdAt = Time.time;
        if (createAidKitSound != null)
        {
            createAidKitSound.Play();
        }
    }

    public void OnCollisionWithUnit(UnitController unit)
    {
        if (
            !isDead
            && (Time.time - createdAt >= pickUpTimeout) 
            && unit.IsAlive() 
            && unit.CanPickUpItem(this)
        )
        {
            Die();
            actions.PickUpItem(unit, this);
        }
    }

    public void Die()
    {
        isDead = true;

        if (takeAidKitSound != null)
        {
            takeAidKitSound.Play();
        }

        animator.Play("Die"); 
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
