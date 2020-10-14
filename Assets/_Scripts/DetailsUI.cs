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
        nameText.text = details.characterName;

        string text = "" + details.health.ToString() + "/" + details.healthMax.ToString();
        healthText.SetText(text, true);

        //Debug.Log(slider.value);
        slider.maxValue = details.healthMax;
        slider.value = details.health;

    }

    public void UpdateHP(float health)
    {
        slider.value = health;
        healthText.text = "" + health.ToString() + "/" + slider.maxValue.ToString();
    }


}
