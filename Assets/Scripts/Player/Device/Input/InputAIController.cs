using UnityEngine;

public class InputAIController : MonoBehaviour
{
    public static readonly int EASY = 8;
    public static readonly int NORMAL = 4;
    public static readonly int HARD = 1;
    public static int difficulty = NORMAL;

    static readonly float aiAxisUpdateTimeoutHard = 0.05f;
    static readonly float aiTargetUpdateTimeoutHard = 0.1f;
    static readonly float aiAxisUpdateTimeoutNormal = 0.25f;
    static readonly float aiTargetUpdateTimeoutNormal = 0.4f;
    static readonly float aiAxisUpdateTimeoutEasy = 0.35f;
    static readonly float aiTargetUpdateTimeoutEasy = 0.65f;
    static float aiAxisUpdateTimeout = aiAxisUpdateTimeoutNormal;
    static float aiTargetUpdateTimeout = aiTargetUpdateTimeoutNormal;

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

    public Axis GetUpdatedAxis()
    {
        return UpdateAxis();
    }

    Axis UpdateAxis()
    {

        if (Time.time - aiAxisUpdatedAt < aiAxisUpdateTimeout)
        {
            return axis;
        }

        aiAxisUpdatedAt = Time.time;

        if (Time.time - aiTargetUpdatedAt > aiTargetUpdateTimeout)
        {
            aiTargetUpdatedAt = Time.time;
            UpdateTarget();
        }

        var randomFactor = 0.1f;
        aiAxisVelocity.x = Random.Range(-randomFactor * difficulty, randomFactor * difficulty);
        aiAxisVelocity.z = Random.Range(-randomFactor * difficulty, randomFactor * difficulty);

        var distance = Vector3.Distance(target, transform.position) / 4f;
        axis.SetX((target.x - transform.position.x) * distance + aiAxisVelocity.x);
        axis.SetY((target.z - transform.position.z) * distance + aiAxisVelocity.z);
        axis.SetAction(distance > 2f && Random.Range(0f, 1f) >= 0.9f ? 1f : 0f);
        axis.SetAction2(isAttack ? 1 : 0);

        return axis;
    }

    void UpdateTarget()
    {
        target = Vector3.zero;
        isAttack = false;

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
                if (targetUnit.IsSameTeam(unit))
                {
                    continue;
                }

                if (targetUnit.IsAlive() && targetUnit != unit)
                {
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

    public static void ToggleDifficulty()
    {
        if (difficulty == EASY)
        {
            difficulty = NORMAL;
            aiAxisUpdateTimeout = aiAxisUpdateTimeoutNormal;
            aiTargetUpdateTimeout = aiTargetUpdateTimeoutNormal;
        }
        else if (difficulty == NORMAL)
        {
            difficulty = HARD;
            aiAxisUpdateTimeout = aiAxisUpdateTimeoutHard;
            aiTargetUpdateTimeout = aiTargetUpdateTimeoutHard;
        }
        else
        {
            difficulty = EASY;
            aiAxisUpdateTimeout = aiAxisUpdateTimeoutEasy;
            aiTargetUpdateTimeout = aiTargetUpdateTimeoutEasy;
        }
    }
}
