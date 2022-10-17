using UnityEngine;

public class AnimatedUnitController : MonoBehaviour
{
    UnitController unit;

    private void Start()
    {
        unit = transform.parent.GetComponent<UnitController>();
    }

    /** Called from animation: Attack01 **/
    public void Attack()
    {
        unit.Attack();
    }
}
