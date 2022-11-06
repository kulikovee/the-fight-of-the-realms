﻿using UnityEngine;

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

            foreach (var ally in teammatesToHeal)
            {
                if (selfEffectPrefab != null)
                {
                    CreateEffect(selfEffectPrefab, ally.gameObject);
                }

                ally.AddHp(unit.maxMana / 1.5f);
            }
        }
    }
}
