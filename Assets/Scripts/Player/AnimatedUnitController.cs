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

    /** Called from animation: Attack01 **/
    public void AttackStarted()
    {
        unit.SetIsAttack(true);
    }

    /** Called from animation: Attack01 **/
    public void AttackFinished()
    {
        unit.SetIsAttack(false);
    }

    /** Called from animation: GetHit **/
    public void HitStarted()
    {
        AttackFinished();
        unit.SetIsHit(true);
    }

    /** Called from animation: GetHit **/
    public void HitFinished()
    {
        unit.SetIsHit(false);
    }

    /** Called from animation: Die **/
    public void DieStarted()
    {
        HitFinished();
        AttackFinished();
    }
}
