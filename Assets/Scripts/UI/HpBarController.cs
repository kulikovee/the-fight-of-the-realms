using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    public Image hpBarImage;

    public void UpdateValue(float hpPercentage)
    {
        hpBarImage.fillAmount = hpPercentage;
    }
}