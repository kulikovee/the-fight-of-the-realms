using System.Collections;
using UnityEngine;

public class Fire : Ability
{
    float castRadius = 3f;
    float castDistance = 1f;

    Fire()
    {
        title = "Fire";
        manaRequired = 50f;
    }

    void Start()
    {
        ActionsController.OnRoundEnd += OnRoundEnd;
    }

    void OnDestroy()
    {
        ActionsController.OnRoundEnd -= OnRoundEnd;
    }

    void OnRoundEnd()
    {
        // To not make damage in the Next Round
        StopAllCoroutines();
    }

    override protected void CastApply()
    {
        unit.AddMana(-manaRequired);

        if (targetEffectPrefab != null)
        {
            CreateEffect(targetEffectPrefab, unit.gameObject);
        }

        if (effectSound != null)
        {
            effectSound.Play();
        }

        StartCoroutine(DoDamageDelayed(0.5f));
        StartCoroutine(DoDamageDelayed(1f));
        StartCoroutine(DoDamageDelayed(1.5f));
    }

    IEnumerator DoDamageDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoDamage(GetForwardAtDistance(castDistance));
    }

    void DoDamage(Vector3 atPosition)
    {
        var aliveEnemies = GetAliveEnemies(castRadius, atPosition);
        if (aliveEnemies.Length > 0)
        {
            foreach (var enemy in aliveEnemies)
            {
                if (affectedEffectPrefab != null)
                {
                    CreateEffect(affectedEffectPrefab, enemy.gameObject);
                }

                enemy.GetHit(unit, unit.maxMana / 3f);
            }
        }
    }
}
