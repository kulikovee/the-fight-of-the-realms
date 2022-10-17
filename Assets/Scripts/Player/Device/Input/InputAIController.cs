using UnityEngine;

public class InputAIController : MonoBehaviour
{
    const float aiAxisUpdateTimeout = 0.25f;
    const float aiTargetUpdateTimeout = 0.4f;

    readonly Axis axis = new();
    float aiTargetUpdatedAt = 0f;
    float aiAxisUpdatedAt = 0f;
    Vector3 aiAxisVelocity = new(0, 0, 0);
    Vector3 target = Vector3.zero;
    bool isAttack = false;
    PlayerController player;
    PlayerController[] players;
    AidKitController aid;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        players = GameObject.FindObjectsOfType<PlayerController>();
        aid = GameObject.FindObjectOfType<AidKitController>();
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

        aiAxisVelocity.x = Random.Range(-0.4f, 0.4f);
        aiAxisVelocity.z = Random.Range(-0.4f, 0.4f);

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

        Vector3 position = transform.position;
        
        // To take First Aid
        if (!aid.isDead)
        {
            isAttack = true;
            target = aid.transform.position;
        }
        else
        {
            // To attack Players
            foreach (var targetPlayer in players)
            {
                if (targetPlayer.GetUnit().IsAlive() && targetPlayer != player)
                {
                    var playerPosition = targetPlayer.transform.position;
                    var distanceToPlayer = Vector3.Distance(position, playerPosition);
                    var distanceToTarget = Vector3.Distance(position, target);

                    if (target == Vector3.zero || distanceToPlayer < distanceToTarget)
                    {
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
}
