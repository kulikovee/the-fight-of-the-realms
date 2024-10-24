﻿using System.Collections.Generic;
using UnityEngine;

public class AnimatedUnitController : MonoBehaviour
{
    public Animator animator;
    public List<AudioSource> attackWaveSounds;
    public List<AudioSource> attackHitSounds;
    public AudioSource notEnoughManaSound;

    // <Controlled by AnimatedUnitEvents>
    public bool isCastSpellInProgress = false;
    public bool isSpecialAttackInProgress = false;
    public bool isAttackInProgress = false;
    public bool isHitInProgress = false;
    public bool isHit = false;
    // </Controlled by AnimatedUnitEvents>

    NotificationController notifications;

    float lastAttackStartedAt = 0f;
    float lastSpecialAttackStartedAt = 0f;
    float lastCastSpellStartedAt = 0f;
    readonly float lastAttackStartedTimeout = 0.2f;
    readonly float lastSpecialAttackStartedTimeout = 0.2f;
    readonly float lastCastSpellStartedTimeout = 0.2f;

    DeviceController device;
    UnitController unit;

    void Start()
    {
        unit = GetComponent<UnitController>();
        device = GetComponent<DeviceController>();
        notifications = NotificationController.GetNotifications();
    }

    void Update()
    {
        var axis = device.GetAxis();
        var isDeadAnimation = !unit.IsAlive();
        var isPlayingAnimation = isDeadAnimation;

        var isHitAnimation = !isPlayingAnimation && unit.isStunOnHit && (isHitInProgress || isHit);
        isPlayingAnimation |= isHitAnimation;

        var isAttackAnimation = !isPlayingAnimation && (isAttackInProgress || IsAttackPressed());
        isPlayingAnimation |= isAttackAnimation;

        var isSpecialAttackAnimation = !isPlayingAnimation && (isSpecialAttackInProgress || IsSpecialAttackPressed());
        isPlayingAnimation |= isSpecialAttackAnimation;

        var isCastSpellAnimation = !isPlayingAnimation && (isCastSpellInProgress || IsCastSpellPressed());
        isPlayingAnimation |= isCastSpellAnimation;

        var isRunAnimation = !isPlayingAnimation && (Mathf.Abs(axis.GetX()) + Mathf.Abs(axis.GetY()) > 0.2f);

        animator.SetBool("die", isDeadAnimation);
        animator.SetBool("run", isRunAnimation);
        animator.SetBool("hit", isHitAnimation);
        animator.SetBool("attack", isAttackAnimation);
        animator.SetBool("cast", isCastSpellAnimation);
        animator.SetBool("specialAttack", isSpecialAttackAnimation);

        isHit = false;
    }

    bool IsAttackPressed()
    {
        var axis = device.GetAxis();
        var isAttackPressed = axis.GetButtonX() != 0f;
        if (isAttackPressed)
        {
            lastAttackStartedAt = Time.time;
        }
        return lastAttackStartedAt != 0f && Time.time - lastAttackStartedAt <= lastAttackStartedTimeout;
    }

    bool IsSpecialAttackPressed()
    {
        var axis = device.GetAxis();
        var isSpecialAttackPressed = axis.GetButtonO() != 0f;
        if (isSpecialAttackPressed && unit.IsEnoughManaToSecondAbility())
        {
            lastSpecialAttackStartedAt = Time.time;
        }

        var isRecentlyPressed = lastSpecialAttackStartedAt != 0f && Time.time - lastSpecialAttackStartedAt <= lastSpecialAttackStartedTimeout;

        if (
            !isRecentlyPressed
            && isSpecialAttackPressed
            && !unit.IsEnoughManaToSecondAbility()
            && notEnoughManaSound != null
            && !notEnoughManaSound.isPlaying
        )
        {
            notifications.Notify(unit, "Not enough mana");
            notEnoughManaSound.Play();
        }

        return isRecentlyPressed;
    }

    bool IsCastSpellPressed()
    {
        var axis = device.GetAxis();
        var isCastSpellPressed = axis.GetButtonY() != 0f;

        if (isCastSpellPressed && unit.IsEnoughManaToMainAbility())
        {
            lastCastSpellStartedAt = Time.time;
        }

        var isRecentlyPressed = lastCastSpellStartedAt != 0f && Time.time - lastCastSpellStartedAt <= lastCastSpellStartedTimeout;

        if (
            !isRecentlyPressed
            && isCastSpellPressed
            && !unit.IsEnoughManaToMainAbility()
            && notEnoughManaSound != null
            && !notEnoughManaSound.isPlaying
        )
        {
            notifications.Notify(unit, "Not enough mana");
            notEnoughManaSound.Play();
        }

        return isRecentlyPressed;
    }

    public void PlayAttackWaveSound()
    {
        var randomSound = Random.Range(0, attackWaveSounds.Count);
        if (attackWaveSounds.Count > 0 && attackWaveSounds[randomSound] != null)
        {
            PlaySound(attackWaveSounds[randomSound]);
        }
    }

    public void PlayAttackHitSound()
    {
        var randomSound = Random.Range(0, attackHitSounds.Count);
        if (attackHitSounds.Count > 0 && attackHitSounds[randomSound] != null)
        {
            PlaySound(attackHitSounds[Random.Range(0, attackHitSounds.Count)]);
        }
    }

    public void Hit()
    {
        isHit = true;
    }

    void PlaySound(AudioSource sound)
    {
        if (!sound.isPlaying)
        {
            sound.Play();
        }
    }

    /** Called from AnimatedUnitEvents => Attack01 **/
    public void Attack()
    {
        unit.Attack();
    }

    /** Called from AnimatedUnitEvents => Attack02 **/
    public void SpecialAttack()
    {
        if (animator.GetBool("specialAttack"))
        {
            unit.SecondAbility();
        }
    }

    /** Called from AnimatedUnitEvents => Revive animation **/
    public void CastSpell()
    {
        unit.MainAbility();
    }
}
