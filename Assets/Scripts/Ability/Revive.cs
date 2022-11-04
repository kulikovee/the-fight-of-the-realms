using UnityEngine;

public class Revive : Ability
{
    [System.NonSerialized]
    public new string title = "Revive";
    [System.NonSerialized]
    protected new float manaRequired = 50f;
    float castRadius = 3f;

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

            foreach (var unit in teammatesToRevive)
            {
                if (targetEffectPrefab != null)
                {
                    CreateEffect(targetEffectPrefab, unit.gameObject);
                }

                unit.AddHp(unit.maxMana);
            }
        }
    }
}
