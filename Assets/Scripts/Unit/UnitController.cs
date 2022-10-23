using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public Image hpBarImage;

    public float maxHp = 150f;
    public float attackPower = 25f;
    public float specialAttackPower = 40f;
    public float speed = 1f;
    public string team = "";
    public bool canPickUpItems = true;
    public bool isStunOnHit = true;
    public GameObject bombPrefab;

    DeviceController device;
    AnimatedUnitController animatedUnit;
    KinematicCharacterAdapter characterAdapter;

    ActionsController actions;
    Vector3 defaultPosition;
    Quaternion defaultRotation;

    float hp = 0;
    float attackRadius = 0.8f;
    float attackDistance = 0.5f;

    void Start()
    {
        ActionsController.OnRoundEnd += FreezeAndResetPosition;
        ActionsController.OnRoundStart += Unfreeze;
        ActionsController.OnEndGame += UnselectResetAndFreeze;

        characterAdapter = GetComponent<KinematicCharacterAdapter>();
        device = GetComponent<DeviceController>();
        animatedUnit = GetComponent<AnimatedUnitController>();
        actions = ActionsController.GetActions();

        hp = maxHp;
        defaultPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        defaultRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    void OnDestroy()
    {
        ActionsController.OnRoundEnd -= FreezeAndResetPosition;
        ActionsController.OnRoundStart -= Unfreeze;
        ActionsController.OnEndGame -= UnselectResetAndFreeze;
    }

    void Update()
    {
    }

    public void SpecialAttack()
    {
        if (bombPrefab != null)
        {
            var createBombPosition = transform.position + transform.forward + Vector3.up;
            var bomb = Instantiate(bombPrefab, createBombPosition, Quaternion.identity);
            var bombController = bomb.GetComponent<Bomb>();
            if (bombController != null)
            {
                bombController.Throw(this, transform.forward * 400f + Vector3.up * 3f);
            }
        }
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
                unit.GetHit(this, attackPower);
                attackedUnits.Add(unit);
            }
        }

        if (animatedUnit != null)
        {
            if (attackedUnits.Count > 0)
            {
                animatedUnit.PlayAttackHitSound();
            } else
            {
                animatedUnit.PlayAttackWaveSound();
            }
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

    public void GetHit(UnitController attacker, float damage)
    {
        if (IsAlive())
        {
            AddHp(-damage);

            if (IsAlive())
            {
                if (animatedUnit != null)
                {
                    animatedUnit.Hit();
                }
            }
            else
            {
                Die(attacker);
            }
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


    void Die(UnitController killer)
    {
        actions.UnitKilled(this, killer);
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

    void ResetHpPositionRotation()
    {
        RestoreHp();
        characterAdapter.SetPosition(defaultPosition);
        characterAdapter.SetRotation(defaultRotation);
        device.GetAxis().ResetAxis();
    }

    void SetFrozen(bool frozen)
    {
        device.SetFrozen(frozen);
    }

    void FreezeAndResetPosition()
    {
        SetFrozen(true);
        ResetHpPositionRotation();
    }

    void Unfreeze()
    {
        SetFrozen(false);
    }

    void UnselectResetAndFreeze()
    {
        GetDevice().ResetDeviceId();
        SetFrozen(true);
        ResetHpPositionRotation();
    }
}
