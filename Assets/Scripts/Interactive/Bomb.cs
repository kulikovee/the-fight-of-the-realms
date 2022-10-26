using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject particles;
    public GameObject bomb;

    float startAt = 0f;
    bool isExploded = false;
    AudioSource explodeSound;
    UnitController ownerUnit;
    UnitController[] units;
    readonly float explodeTimeout = 1f;
    readonly float destroyTimeout = 1.45f;

    void Start()
    {
        ActionsController.OnRoundEnd += Destroy;
        units = GameObject.FindObjectsOfType<UnitController>();
        explodeSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (startAt != 0f)
        {
            if (!isExploded && Time.time - startAt > explodeTimeout)
            {
                isExploded = true;
                explodeSound.Play();
                particles.SetActive(true);
                bomb.SetActive(false);
                DoDamage();
            } else if (Time.time - startAt > destroyTimeout)
            {
                Destroy();
            }
        }
    }

    void OnDestroy()
    {
        ActionsController.OnRoundEnd -= Destroy;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    public void Throw(UnitController owner, Vector3 direction)
    {
        ownerUnit = owner;
        GetComponent<Rigidbody>().AddForce(direction);
        startAt = Time.time;
    }

    public void DoDamage()
    {
        foreach (var unit in units)
        {
            if (
                !unit.IsSameTeam(ownerUnit)
                && unit != ownerUnit
                && unit.IsAlive()
            )
            {
                var distanceMultiplier = 0f;

                if (Vector3.Distance(transform.position, unit.transform.position) < 1.5f) distanceMultiplier += 0.33f;
                if (Vector3.Distance(transform.position, unit.transform.position) < 2f) distanceMultiplier += 0.33f;
                if (Vector3.Distance(transform.position, unit.transform.position) < 3f) distanceMultiplier += 0.33f;

                if (distanceMultiplier > 0f)
                {
                    unit.GetHit(ownerUnit, ownerUnit.specialAttackPower * distanceMultiplier);
                }
            }
        }
    }
}
