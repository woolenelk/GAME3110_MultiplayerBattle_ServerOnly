using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaifuDetails : MonoBehaviour
{
    //[SerializeField]
    //private string characterName;
    //// how much health points they will have

    //[SerializeField]
    //private int healthMax;
    //// defence how much damage they will block from each attack
    //[SerializeField]
    //private int defence;
    //// attack is how much damage they will deal
    //[SerializeField]
    //private int attack;
    //// love dictates how much they will recover with the rest action
    //[SerializeField]
    //private int love;

    //[SerializeField]
    //public int[] statBuffs = { 0, 0, 0, 0 };



    //public string CharacterName
    //{
    //    get { return characterName; }
    //}

    //public int HealthMax
    //{
    //    get { return healthMax; }
    //}

    //public int Defence
    //{
    //    get { return defence; }
    //}

    //public int Attack
    //{
    //    get { return attack; }
    //}

    //public int Love
    //{
    //    get { return love; }
    //}

    //public int [] StatBuffs
    //{
    //    get { return statBuffs; }
    //}

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

    [SerializeField]
    public int[] buffs = new int[(int)BUFF_ARRAY.COUNT];

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
