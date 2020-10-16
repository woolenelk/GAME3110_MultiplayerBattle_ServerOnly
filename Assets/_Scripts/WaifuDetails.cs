using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaifuDetails : MonoBehaviour
{
    public string characterName;
    // how much health points they will have
    public int health;
    public int healthMax;
    // defence how much damage they will block from each attack
    public int defence;
    // attack is how much damage they will deal
    public int attack;
    // love dictates how much they will recover with the rest action
    public int love;

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
