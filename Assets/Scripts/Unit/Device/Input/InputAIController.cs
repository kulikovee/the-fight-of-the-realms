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
    LevelController level;
    UnitController unit;
    PlayerController player;

    private void Start()
    {
        unit = GetComponent<UnitController>();
        player = GetComponent<PlayerController>();
        level = LevelController.GetLevel();
    }

    public Axis GetAxis()
    {
        return axis;
    }

    public void UpdateAxis()
    {
        if (!unit.IsAlive() || Time.time - aiAxisUpdatedAt < aiAxisUpdateTimeout)
        {
            return;
        }

        aiAxisUpdatedAt = Time.time;

        if (Time.time - aiTargetUpdatedAt > aiTargetUpdateTimeout)
        {
            aiTargetUpdatedAt = Time.time;
            UpdateTarget();
        }

        if (target != Vector3.zero)
        {
            var randomFactor = 0.4f;
            aiAxisVelocity.x = Random.Range(-randomFactor, randomFactor);
            aiAxisVelocity.z = Random.Range(-randomFactor, randomFactor);

            var distance = Vector3.Distance(target, transform.position) / 4f;
            axis.SetX((target.x - transform.position.x) * distance + aiAxisVelocity.x);
            axis.SetY((target.z - transform.position.z) * distance + aiAxisVelocity.z);

            var isSurvivingAlly = level.IsSurvival() && unit.IsTeamAlly() && Random.Range(0f, 1f) >= 0.5f;
            var isRandomJump = distance > 2f && Random.Range(0f, 1f) >= 0.9f;
            axis.SetButtonA(isSurvivingAlly || isRandomJump ? 1f : 0f);
            axis.SetButtonX(isAttack ? 1 : 0);
            axis.SetButtonY(IsPlayer() && unit.GetHp() < 65f ? 1 : 0);
        } else
        {
            axis.ResetAxis();
        }
    }

    void UpdateTarget()
    {
        var units = GameObject.FindObjectsOfType<UnitController>();
        var aidKit = GameObject.FindObjectOfType<ItemAidController>();

        target = GetDefaultAiTarget(units);
        isAttack = false;

        var isTargetUpdated = false;
        Vector3 position = transform.position;
        
        // To take First Aid
        if (!level.IsPlatformer() && aidKit != null && unit.canPickUpItems)
        {
            isAttack = true;
            target = aidKit.transform.position;
        }
        else
        {
            foreach (var enemyUnit in units)
            {
                if (enemyUnit.IsSameTeam(unit) || !enemyUnit.IsAlive() || enemyUnit == unit)
                {
                    continue;
                }

                var enemyUnitPosition = enemyUnit.transform.position;
                var distanceToEnemyUnit = Vector3.Distance(position, enemyUnitPosition);
                var distanceToTarget = Vector3.Distance(position, target);

                if (
                    distanceToEnemyUnit < (level.IsPlatformer() ? 5f : 25f)
                    && (!isTargetUpdated || distanceToEnemyUnit < distanceToTarget)
                )
                {
                    isTargetUpdated = true;
                    target = enemyUnitPosition;

                    if (distanceToEnemyUnit < 2f)
                    {
                        isAttack = true;
                    }
                }
            }

            if (level.IsSurvival() && isTargetUpdated && unit.IsTeamAlly())
            {
                target = transform.position - target;
            }
        }

        if (!level.IsPlatformer())
        {
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

    public Vector3 GetDefaultAiTarget(UnitController[] units)
    {
        var defaultTarget = Vector3.zero;

        if (level.IsPlatformer())
        {
            if (unit.IsTeamAlly())
            {
                foreach (var ally in units)
                {
                    if (!ally.IsSameTeam(unit) || !ally.IsAlive() || ally == unit)
                    {
                        continue;
                    }

                    if (ally.GetDevice().IsSelected())
                    {
                        var distanceToAlly = Vector3.Distance(transform.position, ally.transform.position);
                        var distanceToTarget = Vector3.Distance(transform.position, defaultTarget);

                        if (defaultTarget == Vector3.zero || distanceToAlly < distanceToTarget)
                        {
                            defaultTarget = ally.transform.position - Vector3.right;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var respawnPoint in level.arenaRabbitRespawns)
            {
                var distanceToAidRespawn = Vector3.Distance(unit.transform.position, respawnPoint);
                var distanceToTarget = Vector3.Distance(unit.transform.position, defaultTarget);
                if (defaultTarget == Vector3.zero || distanceToAidRespawn < distanceToTarget)
                {
                    defaultTarget = respawnPoint;
                }
            }
        }

        return defaultTarget;
    }

    bool IsPlayer()
    {
        return player != null;
    }
}
