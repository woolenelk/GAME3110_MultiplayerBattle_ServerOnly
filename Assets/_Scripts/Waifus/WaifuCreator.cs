using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Waifu", menuName = "ScriptableObjects/Waifu", order = 3)]
public class WaifuCreator : ScriptableObject
{
    //[SerializeField]
    //public string characterName;
    //// how much health points they will have
    //[SerializeField]
    //public int health;
    //[SerializeField]
    //public int healthMax;
    //// defence how much damage they will block from each attack
    //[SerializeField]
    //public int defence;
    //// attack is how much damage they will deal
    //[SerializeField]
    //public int attack;
    //// love dictates how much they will recover with the rest action
    //[SerializeField]
    //public int love;


    public Sprite characterImage;

    [SerializeField]
    private string characterName;
    //// how much health points they will have
    //[SerializeField]
    //private int health;
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
    private AbilityList myAbilities;
    public string CharacterName
    {
        get { return characterName; }
    }

    //public int Health
    //{
    //    get { return health; }
        
    //}

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

    public AbilityList MyAbilties
    {
        get { return myAbilities; }
    }

}
