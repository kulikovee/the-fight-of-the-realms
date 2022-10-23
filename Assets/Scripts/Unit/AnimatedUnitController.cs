using System.Collections.Generic;
using UnityEngine;

public class AnimatedUnitController : MonoBehaviour
{
    public Animator animator;
    public List<AudioSource> attackWaveSounds;
    public List<AudioSource> attackHitSounds;
    // Controlled by AnimatedUnitEvents
    public bool isSpecialAttackInProgress = false;
    public bool isAttackInProgress = false;
    public bool isHitInProgress = false;
    public bool isHit = false;

    float lastAttackStartedAt = 0f;
    float lastSpecialAttackStartedAt = 0f;
    readonly float lastAttackStartedTimeout = 0.2f;
    readonly float lastSpecialAttackStartedTimeout = 0.2f;

    DeviceController device;
    UnitController unit;

    void Start()
    {
        unit = GetComponent<UnitController>();
        device = GetComponent<DeviceController>();
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

        var isRunAnimation = !isPlayingAnimation && (Mathf.Abs(axis.GetX()) + Mathf.Abs(axis.GetY()) > 0.2f);

        animator.SetBool("die", isDeadAnimation);
        animator.SetBool("run", isRunAnimation);
        animator.SetBool("hit", isHitAnimation);
        animator.SetBool("attack", isAttackAnimation);
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
        if (isSpecialAttackPressed)
        {
            lastSpecialAttackStartedAt = Time.time;
        }
        return lastSpecialAttackStartedAt != 0f && Time.time - lastSpecialAttackStartedAt <= lastSpecialAttackStartedTimeout;
    }

    public void PlayAttackWaveSound()
    {
        PlaySound(attackWaveSounds[Random.Range(0, attackWaveSounds.Count)]);
    }

    public void PlayAttackHitSound()
    {
        PlaySound(attackHitSounds[Random.Range(0, attackHitSounds.Count)]);
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

    /** Called from animation: Attack01 **/
    public void Attack()
    {
        unit.Attack();
    }

    /** Called from animation: Attack02 **/
    public void SpecialAttack()
    {
        if (animator.GetBool("specialAttack"))
        {
            unit.SpecialAttack();
        }
    }
}
