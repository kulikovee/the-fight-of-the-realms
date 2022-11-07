using UnityEngine;

public class Heal : Ability
{
    float castDistance = 0.5f;
    float castRadius = 2f;

    Heal()
    {
        title = "Heal";
        manaRequired = 40f;
    }

    override protected void CastApply()
    {
        var healAtPosition = GetForwardAtDistance(castDistance);
        var teammatesToHeal = GetAliveAllies(castRadius, healAtPosition);

        if (teammatesToHeal.Length > 0)
        {
            unit.AddMana(-manaRequired);

            if (targetEffectPrefab != null)
            {
                CreateEffect(targetEffectPrefab, healAtPosition);
            }

            if (effectSound != null)
            {
                effectSound.Play();
            }

            var restoreHp = unit.maxMana / 1.5f;

            foreach (var ally in teammatesToHeal)
            {
                if (affectedEffectPrefab != null)
                {
                    CreateEffect(affectedEffectPrefab, ally.gameObject);
                }

                ally.AddHp(restoreHp);
            }
        }
    }
}
