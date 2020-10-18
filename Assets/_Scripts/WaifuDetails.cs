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

    [SerializeField]
    public int[] buffs = new int[(int)BUFF_ARRAY.COUNT];

    public SpriteRenderer waifuSprite;

    public bool TakeDamage( int damage)
    {
        //Debug.Log("Damage in  : " + damage);
        //Debug.Log("Defence    : " + waifu.Defence);
        float modifiedDefence = waifu.Defence * (1 + (0.05f * buffs[(int)BUFF_ARRAY.DEFENCE]));
        float defencePercent = (1.0f - (modifiedDefence / (150.0f + modifiedDefence)));
        damage = (int)(damage * defencePercent);
        //Debug.Log("Damage out : " + damage);
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            return true;
        }
        return false;
    }
    
    public bool Recoil ( int Damage)
    {
        Health -= Damage;

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
