using UnityEngine;

public class Revive : Ability
{
    float castRadius = 3f;

    Revive()
    {
        title = "Revive";
        manaRequired = 70f;
    }

    override protected void CastApply()
    {
        var teammatesToRevive = GetDeadAllies(castRadius);

        if (teammatesToRevive.Length > 0)
        {
            unit.AddMana(-manaRequired);

            if (selfEffectPrefab != null)
            {
                CreateEffect(selfEffectPrefab, gameObject);
            }

            if (effectSound != null)
            {
                effectSound.Play();
            }

            foreach (var ally in teammatesToRevive)
            {
                if (targetEffectPrefab != null)
                {
                    CreateEffect(targetEffectPrefab, ally.gameObject);
                }

                ally.AddHp(unit.maxMana);
            }
        }
    }
}
