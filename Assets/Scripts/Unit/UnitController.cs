using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public Image hpBarImage;
    public Image hpBarImageUI;
    public Image manaBarImage;
    public Image manaBarImageUI;

    public float maxHp = 150f;
    public float maxMana = 50f;
    public float attackPower = 25f;
    public float specialAttackPower = 40f;
    public float attackRadius = 0.8f;
    public float speed = 1f;
    public string team = "";
    public bool canPickUpItems = true;
    public bool isStunOnHit = true;
    public GameObject bombPrefab;
    public GameObject revivePrefab;
    public GameObject revivedPrefab;
    public AudioSource reviveSound;

    DeviceController device;
    AnimatedUnitController animatedUnit;
    KinematicCharacterAdapter characterAdapter;

    ActionsController actions;
    Vector3 defaultPosition;
    Quaternion defaultRotation;

    float hp = 0;
    float mana = 0;
    float attackDistance = 0.5f;
    float manaRestoreTimeout = 0.125f;
    float manaRestoredAt = 0;
    public float specialAttackManaCost = 25f;
    public float spellManaCost = 50f;

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
        mana = maxMana;
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
        if (IsAlive() && mana < maxMana && Time.time - manaRestoredAt > manaRestoreTimeout)
        {
            manaRestoredAt = Time.time;
            AddMana(0.25f);
        }

        if (transform.position.y < -50f)
        {
            SetHp(0);
            Die(this);
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

    public void SpecialAttack()
    {
        if (bombPrefab != null)
        {
            AddMana(-specialAttackManaCost);

            var createBombPosition = transform.position + transform.forward + Vector3.up;
            var bomb = Instantiate(bombPrefab, createBombPosition, Quaternion.identity);
            var bombController = bomb.GetComponent<Bomb>();
            if (bombController != null)
            {
                bombController.Throw(this, transform.forward * 400f + Vector3.up * 3f);
            }
        }
    }

    public void CastSpell()
    {
        var teammatesToRevive = new List<UnitController>();

        foreach (var unit in GameObject.FindObjectsOfType<UnitController>())
        {
            if (
                IsSameTeam(unit) 
                && !unit.IsAlive()
                && Vector3.Distance(unit.transform.position, transform.position) < 3
            )
            {
                teammatesToRevive.Add(unit);
            }
        }

        if (teammatesToRevive.Count > 0)
        {
            AddMana(-spellManaCost);

            if (revivePrefab != null)
            {
                var reviveEffect = Instantiate(revivePrefab, transform.position, Quaternion.identity);
                reviveEffect.transform.parent = gameObject.transform;
            }

            if (reviveSound != null)
            {
                reviveSound.Play();
            }

            foreach(var unit in teammatesToRevive)
            {
                if (revivedPrefab != null)
                {
                    var revivedEffect = Instantiate(revivedPrefab, unit.transform.position, Quaternion.identity);
                    revivedEffect.transform.parent = unit.transform;
                }

                unit.AddHp(maxMana);
            }
        }
    }

    public bool IsEnoughManaToSpell()
    {
        return mana >= spellManaCost;
    }

    public bool IsEnoughManaToSpecialAttack()
    {
        return mana >= specialAttackManaCost;
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
        SetHp(Mathf.Clamp(hp + addAmount, 0f, maxHp));
    }

    public void AddMana(float addAmount)
    {
        SetMana(Mathf.Clamp(mana + addAmount, 0f, maxMana));
    }

    public DeviceController GetDevice()
    {
        return device;
    }

    public bool IsSameTeam(UnitController unit)
    {
        return team != "" && unit.team != "" && unit.team == team;
    }

    public float GetHp()
    {
        return hp;
    }

    public float GetMana()
    {
        return mana;
    }

    void Die(UnitController killer)
    {
        actions.UnitKilled(this, killer);
    }

    void RestoreHpAndMana()
    {
        SetHp(maxHp);
        SetMana(maxMana);
    }

    void SetHp(float amount)
    {
        hp = amount;
        hpBarImage.rectTransform.anchorMax = new Vector2(
            hp / maxHp,
            hpBarImage.rectTransform.anchorMax.y
        );

        if (hpBarImageUI != null)
        {
            hpBarImageUI.rectTransform.anchorMax = new Vector2(
                hp / maxHp,
                hpBarImageUI.rectTransform.anchorMax.y
            );
        }
    }

    void SetMana(float amount)
    {
        mana = amount;
        manaBarImage.rectTransform.anchorMax = new Vector2(
            mana / maxMana,
            manaBarImage.rectTransform.anchorMax.y
        );

        if (manaBarImageUI != null)
        {
            manaBarImageUI.rectTransform.anchorMax = new Vector2(
                mana / maxMana,
                manaBarImageUI.rectTransform.anchorMax.y
            );
        }
    }

    void ResetHpPositionRotation()
    {
        RestoreHpAndMana();
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
