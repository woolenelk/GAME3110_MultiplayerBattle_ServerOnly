using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaifuDetails : MonoBehaviour
{
    [SerializeField]
    private string characterName;
    // how much health points they will have
    [SerializeField]
    private int health;
    [SerializeField]
    private int healthMax;
    // defence how much damage they will block from each attack
    [SerializeField]
    private int defence;
    // attack is how much damage they will deal
    [SerializeField]
    private int attack;
    // love dictates how much they will recover with the rest action
    [SerializeField]
    private int love;

    [SerializeField]
    public int[] statBuffs = { 0, 0, 0, 0 };

    public string CharacterName
    {
        get { return characterName; }
    }

    public int Health
    {
        get { return health; }
    }

    public int HealthMax
    {
        get { return healthMax; }
    }

    public int Defence
    {
        get { return defence; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Love
    {
        get { return love; }
    }

    public int [] StatBuffs
    {
        get { return statBuffs; }
    }

    public AbilityList masterAbilityList;

    Ability[] MyAbilities = {  };

    public bool TakeDamage( int damage)
    {
        health -= damage;

        if (health <= 0)
            return true;

        return false;
    }
    
    public void Rest (int heal)
    {
        health += heal;
        if (health > healthMax)
            health = healthMax;
    }
}
