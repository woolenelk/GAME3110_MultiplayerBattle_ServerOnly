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
        nameText.text = details.CharacterName;

        string text = "" + details.Health.ToString() + "/" + details.HealthMax.ToString();
        healthText.SetText(text, true);

        //Debug.Log(slider.value);
        slider.maxValue = details.HealthMax;
        slider.value = details.Health;

    }

    public void UpdateHP(float health)
    {
        slider.value = health;
        healthText.text = "" + health.ToString() + "/" + slider.maxValue.ToString();
    }


}
