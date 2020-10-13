using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        health = healthMax;
        TextUI = TextUI.GetComponent<TextMeshProUGUI>();
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
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

}
