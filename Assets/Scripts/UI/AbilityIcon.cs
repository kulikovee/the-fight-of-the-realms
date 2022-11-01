using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    public enum AbilityType
    {
        BOMB,
        REVIVE,
        ATTACK,
        JUMP,
    };

    public UnitController unit;
    public AbilityType abilityType = AbilityType.BOMB;
    public GameObject outline;
    Image icon;

    void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (abilityType == AbilityType.BOMB || abilityType == AbilityType.REVIVE)
        {
            var manaRequired = abilityType == AbilityType.BOMB 
                ? unit.specialAttackManaCost
                : unit.spellManaCost;
            var value = unit.GetMana() < manaRequired
                ? unit.GetMana() / manaRequired
                : 1;

            icon.fillAmount = value;

            if (outline != null)
            {
                outline.SetActive(value >= 1);
            }
        }
    }
}
