using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public Animator animator;
    public AnimationClip attackAnimation;
    public AnimationClip hitAnimation;
    public List<AudioSource> attackWaveSounds;
    public List<AudioSource> attackHitSounds;
    public Image hpBarImage;

    public float maxHp = 150f;
    public float attackPower = 25f;
    public float speed = 1f;
    public string team = "";
    public bool canPickUpItems = true;

    public bool isStunOnHit = true;

    DeviceController device;
    KinematicCharacterAdapter characterAdapter;

    ActionsController actions;
    Vector3 defaultPosition;
    Quaternion defaultRotation;

    float hp = 0;
    float attackRadius = 0.8f;
    float attackDistance = 0.5f;
    bool isAttack = false;
    bool isHit = false;

    void Start()
    {
        ActionsController.OnRoundEnd += FreezeAndResetPosition;
        ActionsController.OnRoundStart += Unfreeze;
        ActionsController.OnEndGame += ResetUnitDeviceAndFreeze;

        characterAdapter = GetComponent<KinematicCharacterAdapter>();
        device = GetComponent<DeviceController>();
        actions = ActionsController.GetActions();

        hp = maxHp;
        defaultPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        defaultRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    void OnDestroy()
    {
        ActionsController.OnRoundEnd -= FreezeAndResetPosition;
        ActionsController.OnRoundStart -= Unfreeze;
        ActionsController.OnEndGame -= ResetUnitDeviceAndFreeze;
    }

    void Update()
    {
        var axis = device.GetUpdatedAxis();
        var isDead = !IsAlive();
        var isPlayingAnimation = isDead;

        var isHitAnimation = !isPlayingAnimation && isStunOnHit && isHit;
        isPlayingAnimation |= isHitAnimation;

        var isAttackAnimation = !isPlayingAnimation && (!isAttack || axis.GetAction2() != 0);
        isPlayingAnimation |= isAttackAnimation;

        var isRunAnimation = !isPlayingAnimation && (Mathf.Abs(axis.GetX()) + Mathf.Abs(axis.GetY()) > 0.2f);

        animator.SetBool("die", isDead);
        animator.SetBool("hit", isHitAnimation);
        animator.SetBool("attack", isAttackAnimation);
        animator.SetBool("run", isRunAnimation);
    }

    public void Attack()
    {
        var attackedUnits = new List<UnitController>() { };
        var attackPoint = transform.position + transform.forward * attackDistance + Vector3.up * 0.5f;
        var colliders = Physics.OverlapSphere(attackPoint, attackRadius);

        foreach (var collider in colliders)
        {
            var unit = collider.gameObject.GetComponent<UnitController>();

            if (unit != null && unit != this && unit.IsAlive() && !IsSameTeam(unit))
            {
                unit.GetHit(attackPower);
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
    
    public bool IsAlive()
    {
        return hp > 0;
    }

    public bool CanPickUpItem(ItemController item)
    {
        return canPickUpItems;
    }

    public void SetPosition(Vector3 position)
    {
        characterAdapter.SetPosition(position);
    }

    void GetHit(float amountHp)
    {
        AddHp(-amountHp);

        if (!IsAlive())
        {
            Die();
        }
    }

    public void AddHp(float addAmount)
    {
        var missingHp = maxHp - hp;
        SetHp(hp + Mathf.Min(addAmount, missingHp));
    }
    public DeviceController GetDevice()
    {
        return device;
    }

    public bool IsSameTeam(UnitController unit)
    {
        return team != "" && unit.team != "" && unit.team == team;
    }

    public void SetIsAttack(bool _isAttack)
    {
        isAttack = _isAttack;
    }

    public void SetIsHit(bool _isHit)
    {
        isHit = _isHit;
    }

    void Die()
    {
    }

    void RestoreHp()
    {
        SetHp(maxHp);
    }

    void SetHp(float amount)
    {
        hp = amount;
        hpBarImage.fillAmount = hp / maxHp;
    }

    void ResetUnit()
    {
        RestoreHp();
        characterAdapter.SetPosition(defaultPosition);
        characterAdapter.SetRotation(defaultRotation);
        device.GetUpdatedAxis().ResetAxis();
    }

    void SetFrozen(bool frozen)
    {
        device.SetFrozen(frozen);
    }

    void FreezeAndResetPosition()
    {
        SetFrozen(true);
        ResetUnit();
    }

    void Unfreeze()
    {
        SetFrozen(false);
    }

    void ResetUnitDeviceAndFreeze()
    {
        GetDevice().ResetDeviceId();
        SetFrozen(true);
        ResetUnit();
    }
}
