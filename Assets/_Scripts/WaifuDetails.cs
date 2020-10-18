using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaifuDetails : MonoBehaviour
{
    
    // how much health points they have
    [SerializeField]
    private int health;
   

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    [SerializeField]
    public WaifuCreator waifu;


    public SpriteRenderer waifuSprite;


    public bool TakeDamage( int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            return true;
        }
        return false;
    }
    
    public void Rest (int heal)
    {
        Health += heal;
        if (Health > waifu.HealthMax)
            Health = waifu.HealthMax;
    }
}
