using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Animator animator;
    public AudioSource createAidKitSound;
    public AudioSource takeAidKitSound;
    public bool isDead = true;

    float pickUpTimeout = 0.3f;
    float createdAt = 0;
    ActionsController actions;

    void Start()
    {
        actions = ActionsController.GetActions();
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
            Destroy();
            actions.PickUpItem(unit, this);
        }
    }

    public void Create(Vector3 position, Quaternion rotation)
    {
        createdAt = Time.time;
        transform.SetPositionAndRotation(position, rotation);
        isDead = false;
        createAidKitSound.Play();
        animator.Play("Show");
    }

    public void Destroy()
    {
        isDead = true;
        takeAidKitSound.Play();
        animator.Play("Die");
    }
}
