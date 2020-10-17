using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public Slider slider;


    public void FillUI(WaifuDetails details)
    {
        WaifuCreator baseDetails = details.waifu; ;

        nameText.text = baseDetails.CharacterName;

        string text = "" + details.Health.ToString() + "/" + baseDetails.HealthMax.ToString();
        healthText.SetText(text, true);

        //Debug.Log(slider.value);
        slider.maxValue = baseDetails.HealthMax;
        slider.value = details.Health;

    }

    public void UpdateHP(float health)
    {
        slider.value = health;
        healthText.text = "" + health.ToString() + "/" + slider.maxValue.ToString();
    }


}
