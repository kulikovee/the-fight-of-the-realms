using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Animator animator;
    public List<AudioSource> attackWaveSounds;
    public List<AudioSource> attackHitSounds;

    private DeviceController device;
    private KinematicCharacterAdapter characterAdapter;
    private ActionsContoller actions;
    private HpBarController hpBar;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private float hp = 100f;
    private float maxHp = 100f;

    private float lastHitAt = 0f;
    private float hitTimeout = 0.25f;

    private float lastAttackAt = 0f;
    private float attackTimeout = 0.375f;
    private float attackPower = 25f;
    private float attackRadius = 0.8f;
    private float attackDistance = 0.5f;

    void Start()
    {
        characterAdapter = GetComponent<KinematicCharacterAdapter>();
        device = GetComponent<DeviceController>();
        hpBar = GetComponent<HpBarController>();
        actions = ActionsContoller.GetActions();
        defaultPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        defaultRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    void Update()
    {
        var axis = device.GetUpdatedAxis();
        var isDead = !IsAlive();
        var isHit = !isDead && IsHitHappend();
        var isAttack = !isDead && !isHit && !IsAttackAvailable() && lastAttackAt != 0f;
        var isRun = !isDead && !isHit && !isAttack && (Mathf.Abs(axis.GetX()) + Mathf.Abs(axis.GetY()) > 0.2f);

        animator.SetBool("die", isDead);
        animator.SetBool("attack", isAttack);
        animator.SetBool("run", isRun);
        animator.SetBool("hit", isHit);

        if (axis.GetAction2() != 0 && !isHit)
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (IsAlive() && IsAttackAvailable())
        {
            lastAttackAt = Time.time;

            var attackedUnits = new List<UnitController>() { };
            var attackPoint = transform.position + transform.forward * attackDistance + Vector3.up;
            var colliders = Physics.OverlapSphere(attackPoint, attackRadius);

            foreach (var collider in colliders)
            {
                var unit = collider.gameObject.GetComponent<UnitController>();

                if (unit != null && unit != this && unit.IsAlive())
                {
                    unit.ReceiveHit(attackPower);
                    attackedUnits.Add(unit);
                }
            }

            foreach (var unit in attackedUnits)
            {
                if (!unit.IsAlive())
                {
                    actions.UnitKilled(unit, this);
                }
            }

            var sound = attackedUnits.Count > 0
                ? attackHitSounds[Random.Range(0, attackHitSounds.Count)]
                : attackWaveSounds[Random.Range(0, attackWaveSounds.Count)];

            if (!sound.isPlaying)
            {
                sound.Play();
            }
        }
    }

    public void ReceiveHit(float amountHp)
    {
        lastHitAt = Time.time;
        AddHp(-amountHp);

        if (!IsAlive())
        {
            Die();
        }
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    public void Die()
    {
        SetFrozen(true);
    }

    public void RestoreHp()
    {
        SetHp(maxHp);
    }

    public void AddHp(float amount)
    {
        SetHp(hp + amount);
    }

    public void SetHp(float amount)
    {
        hp = amount;
        hpBar.UpdateValue(hp / maxHp);
    }

    public bool IsAttackAvailable()
    {
        return Time.time - lastAttackAt >= attackTimeout;
    }

    public bool IsHitHappend()
    {
        return lastHitAt != 0f && Time.time - lastHitAt <= hitTimeout;
    }

    public void ResetUnit()
    {
        RestoreHp();
        characterAdapter.SetPosition(defaultPosition);
        characterAdapter.SetRotation(defaultRotation);
        device.GetUpdatedAxis().ResetAxis();
    }

    public void SetFrozen(bool frozen)
    {
        device.SetFrozen(frozen);
    }

    public DeviceController GetDevice()
    {
        return device;
    }
}
