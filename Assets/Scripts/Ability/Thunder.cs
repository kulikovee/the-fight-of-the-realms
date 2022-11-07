using System.Collections;
using UnityEngine;

public class Thunder : Ability
{
    float castRadius = 3f;
    float castDistance = 1f;

    Thunder()
    {
        title = "Thunder";
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
        var castPosition = GetForwardAtDistance(castDistance);

        unit.AddMana(-manaRequired);

        if (targetEffectPrefab != null)
        {
            CreateEffect(targetEffectPrefab, castPosition);
        }

        if (effectSound != null)
        {
            effectSound.Play();
        }

        StartCoroutine(DoDamageDelayed(castPosition, 0.5f));
        StartCoroutine(DoDamageDelayed(castPosition, 1f));
        StartCoroutine(DoDamageDelayed(castPosition, 2f));
    }

    IEnumerator DoDamageDelayed(Vector3 atPosition, float delay)
    {
        yield return new WaitForSeconds(delay);
        DoDamage(atPosition);
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
