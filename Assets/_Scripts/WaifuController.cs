using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WaifuController 
    : MonoBehaviour
{
    // how much health points they will have
    public int health;
    public int healthMax;
    // defence how much damage they will block from each attack
    public int defence;
    // attack is how much damage they will deal
    public int attack;
    // love dictates how much they will recover with the rest action
    public int love;
    public TextMeshProUGUI TextUI;
    public UnityEngine.UI.Slider slider;
    //public GameObject slider;

    // Start is called before the first frame update
    void Start()
    {
        //health = healthMax;
        //TextUI = TextUI.GetComponent<TextMeshProUGUI>();
        //slider = GameObject.Find("Player Health").GetComponent<UnityEngine.UI.Slider>();
    }

    void Update()
    {
        UpdateHealth();
    }

    public void AbilityAttack1()
    {

    }

    public void AbilityAttack2()
    {

    }

    public void AbilityAttack3()
    {

    }

    public void AbilityGuardUp()
    {

    }

    public void AbilityRest()
    {

    }

    public void UpdateHealth()
    {
        ///health + " / " + healthMax);
        string text = "" + health.ToString() + "/" + healthMax.ToString();
        TextUI.SetText(text,true);

        //Debug.Log(slider.value);
        slider.value = (float)health / (float) healthMax;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

}
