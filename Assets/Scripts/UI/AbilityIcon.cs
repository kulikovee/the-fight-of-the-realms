using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    public enum AbilityType
    {
        SECOND_ABILITY,
        MAIN_ABILITY,
    };

    public UnitController unit;
    public AbilityType abilityType = AbilityType.SECOND_ABILITY;
    public GameObject outline;
    public TextMeshProUGUI title;
    Image icon;

    void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (abilityType == AbilityType.SECOND_ABILITY || abilityType == AbilityType.MAIN_ABILITY)
        {
            if (abilityType == AbilityType.MAIN_ABILITY)
            {
                title.text = unit.GetMainAbility().GetTitle();
                title.rectTransform.sizeDelta = new Vector2(title.text.Length * 16, 30);
            }

            var manaRequired = abilityType == AbilityType.SECOND_ABILITY 
                ? unit.GetSecondAbilityManaRequired()
                : unit.GetMainAbility().GetManaRequired();

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
