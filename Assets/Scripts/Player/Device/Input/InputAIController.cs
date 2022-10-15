using UnityEngine;

public class InputAIController : MonoBehaviour
{
    private const float aiTargetUpdateTimeout = 0.35f;

    private readonly Axis axis = new();
    private float aiLastTargetUpdate = 0f;
    private Vector3 aiControls = new();

    public Axis GetUpdatedAxis()
    {
        return UpdateAxis();
    }

    private Axis UpdateAxis()
    {
        aiControls *= 0.975f;

        if (aiLastTargetUpdate + aiTargetUpdateTimeout > Time.time)
        {
            return axis;
        }

        aiLastTargetUpdate = Time.time;

        float randomDeltaX = Random.Range(-1f, 1f);
        float randomDeltaZ = Random.Range(-1, 1f);

        aiControls += new Vector3(randomDeltaX, randomDeltaZ, 0);

        axis.SetX(aiControls.x);
        axis.SetY(aiControls.y);
        axis.SetAction(Random.Range(0f, 1f) >= 0.5f ? 1f : 0f);

        return axis;
    }
}
