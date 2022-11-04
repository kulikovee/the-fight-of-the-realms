using UnityEngine;

public class Heal : Ability
{
    [System.NonSerialized]
    public new string title = "Heal";
    [System.NonSerialized]
    protected new float manaRequired = 40f;
    float castDistance = 0.5f;
    float castRadius = 2f;

    override protected void CastApply()
    {
        var healAtPosition = transform.position + transform.forward * castDistance + transform.up;
        var teammatesToHeal = GetAliveAllies(castRadius, healAtPosition);

        if (teammatesToHeal.Length > 1)
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

            foreach (var unit in teammatesToHeal)
            {
                if (selfEffectPrefab != null)
                {
                    CreateEffect(selfEffectPrefab, unit.gameObject);
                }

                unit.AddHp(unit.maxMana);
            }
        }
    }
}
