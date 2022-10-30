using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    public enum AbilityType
    {
        BOMB,
        REVIVE,
    };

    public UnitController unit;
    public AbilityType abilityType = AbilityType.BOMB;
    Image icon;

    void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var manaRequired = abilityType == AbilityType.BOMB 
            ? unit.specialAttackManaCost
            : unit.spellManaCost;

        icon.fillAmount = unit.GetMana() < manaRequired
            ? unit.GetMana() / manaRequired
            : 1;
    }
}
