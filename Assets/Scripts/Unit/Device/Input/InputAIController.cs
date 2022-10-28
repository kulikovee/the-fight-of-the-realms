using UnityEngine;

public class InputAIController : MonoBehaviour
{
    static float aiAxisUpdateTimeout = 0.25f;
    static float aiTargetUpdateTimeout = 0.4f;

    readonly Axis axis = new();
    float aiTargetUpdatedAt = 0f;
    float aiAxisUpdatedAt = 0f;
    Vector3 aiAxisVelocity = new(0, 0, 0);
    Vector3 target = Vector3.zero;
    bool isAttack = false;
    UnitController unit;
    UnitController[] units;
    ItemAidController aidKit;

    private void Start()
    {
        unit = GetComponent<UnitController>();
        units = GameObject.FindObjectsOfType<UnitController>();
        aidKit = GameObject.FindObjectOfType<ItemAidController>();
    }

    public Axis GetAxis()
    {
        return axis;
    }

    public void UpdateAxis()
    {

        if (Time.time - aiAxisUpdatedAt < aiAxisUpdateTimeout)
        {
            return;
        }

        aiAxisUpdatedAt = Time.time;

        if (Time.time - aiTargetUpdatedAt > aiTargetUpdateTimeout)
        {
            aiTargetUpdatedAt = Time.time;
            UpdateTarget();
        }

        var randomFactor = 0.4f;
        aiAxisVelocity.x = Random.Range(-randomFactor, randomFactor);
        aiAxisVelocity.z = Random.Range(-randomFactor, randomFactor);

        var distance = Vector3.Distance(target, transform.position) / 4f;
        axis.SetX((target.x - transform.position.x) * distance + aiAxisVelocity.x);
        axis.SetY((target.z - transform.position.z) * distance + aiAxisVelocity.z);
        axis.SetButtonA(distance > 2f && Random.Range(0f, 1f) >= 0.9f ? 1f : 0f);
        axis.SetButtonX(isAttack ? 1 : 0);
    }

    void UpdateTarget()
    {
        target = Vector3.zero;
        isAttack = false;
        units = GameObject.FindObjectsOfType<UnitController>();

        var isTargetUpdated = false;
        Vector3 position = transform.position;

        foreach(var respawnPoint in aidKit.respawnPoints)
        {
            var distanceToAidRespawn = Vector3.Distance(transform.position, respawnPoint);
            var distanceToTarget = Vector3.Distance(transform.position, target);
            if (target == Vector3.zero || distanceToAidRespawn < distanceToTarget)
            {
                target = respawnPoint;
            }
        }
        
        // To take First Aid
        if (!aidKit.GetComponent<ItemController>().isDead && unit.canPickUpItems)
        {
            isAttack = true;
            target = aidKit.transform.position;
        }
        else
        {
            // To attack Players
            foreach (var targetUnit in units)
            {
                if (targetUnit.IsSameTeam(unit) || !targetUnit.IsAlive() || targetUnit == unit)
                {
                    continue;
                }

                var playerPosition = targetUnit.transform.position;
                var distanceToPlayer = Vector3.Distance(position, playerPosition);
                var distanceToTarget = Vector3.Distance(position, target);

                if (distanceToPlayer < 50 && (!isTargetUpdated || distanceToPlayer < distanceToTarget))
                {
                    isTargetUpdated = true;
                    target = playerPosition;

                    if (distanceToPlayer < 2f)
                    {
                        isAttack = true;
                    }
                }
            }
        }

        if (target.y >= 0.4f)
        {
            // If target on top
            if (position.y <= 2)
            {
                // If AI on bottom
                var targetX = position.x;
                var targetZ = position.z;
                var deltaX = 3.6f;
                var deltaZ = 3.8f;
                var groundY = 0.3f;
                var isUpdateZ = Mathf.Abs(position.z) < deltaZ - 0.2f || Mathf.Abs(position.z) > deltaZ + 0.4f;

                if (isUpdateZ && position.y < groundY)
                {
                    targetZ = position.z < 0 ? -deltaZ : deltaZ;

                    if (Mathf.Abs(position.x) < deltaX - 0.2f || Mathf.Abs(position.x) > deltaX + 0.4f)
                    {
                        // AI should go to right / left corner in `x`
                        targetX = position.x < 0 ? -deltaX : deltaX;
                    }
                }
                else
                {
                    // AI should get up to center
                    targetX = 0;
                }


                target = new Vector3(targetX, target.y, targetZ);
            }
        }
    }
}
