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
        CastSpellFinished();
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
        CastSpellFinished();
        AttackFinished();
        animatedUnit.isSpecialAttackInProgress = true;
    }

    /** Called from animation: Attack02 **/
    public void SpecialAttackFinished()
    {
        animatedUnit.isSpecialAttackInProgress = false;
    }

    /** Called from animation: Revive **/
    public void CastSpell()
    {
        animatedUnit.CastSpell();
    }

    /** Called from animation: Attack01 **/
    public void CastSpellStarted()
    {
        SpecialAttackFinished();
        animatedUnit.isCastSpellInProgress = true;
    }

    /** Called from animation: Attack01 **/
    public void CastSpellFinished()
    {
        animatedUnit.isCastSpellInProgress = false;
    }

    /** Called from animation: GetHit **/
    public void HitStarted()
    {
        CastSpellFinished();
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
        CastSpellFinished();
        HitFinished();
        AttackFinished();
        SpecialAttackFinished();
    }
}