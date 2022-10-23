using UnityEngine;

public class AnimatedUnitEvents : MonoBehaviour
{
    AnimatedUnitController animatedUnit;

    void Start()
    {
        animatedUnit = transform.parent.GetComponent<AnimatedUnitController>();
    }

    /** Called from animation: Attack01 **/
    public void Attack()
    {
        animatedUnit.Attack();
    }

    /** Called from animation: Attack01 **/
    public void AttackStarted()
    {
        SpecialAttackFinished();
        animatedUnit.isAttackInProgress = true;
    }

    /** Called from animation: Attack01 **/
    public void AttackFinished()
    {
        animatedUnit.isAttackInProgress = false;
    }

    /** Called from animation: Attack02 **/
    public void SpecialAttack()
    {
        animatedUnit.SpecialAttack();
    }

    /** Called from animation: Attack02 **/
    public void SpecialAttackStarted()
    {
        AttackFinished();
        animatedUnit.isSpecialAttackInProgress = true;
    }

    /** Called from animation: Attack02 **/
    public void SpecialAttackFinished()
    {
        animatedUnit.isSpecialAttackInProgress = false;
    }

    /** Called from animation: GetHit **/
    public void HitStarted()
    {
        AttackFinished();
        SpecialAttackFinished();
        animatedUnit.isHitInProgress = true;
    }

    /** Called from animation: GetHit **/
    public void HitFinished()
    {
        animatedUnit.isHitInProgress = false;
    }

    /** Called from animation: Die **/
    public void DieStarted()
    {
        HitFinished();
        AttackFinished();
        SpecialAttackFinished();
    }
}