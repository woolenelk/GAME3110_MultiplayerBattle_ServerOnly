using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BUFF_ARRAY
{
    ATTACK,
    DEFENCE,
    LOVE,
    COUNT
}

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class Ability : ScriptableObject
{
    [SerializeField]
    private int abilityID;

    [SerializeField]
    private string abilityName;

    [SerializeField]
    private float attackMultiplier;

    [SerializeField]
    private string description = "This is a move";

    [SerializeField]
    private int costHP;


    [SerializeField]
    private int[] selfBuff = new int[3];

    [SerializeField]
    private int[] selfDebuff = new int[3];

    [SerializeField]
    private int[] enemyBuff = new int[3];

    [SerializeField]
    private int[] enemyDebuff = new int[3];

    [SerializeField]
    private float LoveHealMultiplier;

    public int AbilityId
    {
        get { return abilityID; }
        set { abilityID = value; }
    }

    public string AbilityName
    {
        get { return abilityName; }
    }

    public float AttackMultipier
    {
        get { return attackMultiplier; }
    }
    
    public string Description
    {
        get { return description; }
    }

    public int CostHp
    {
        get { return costHP; }
    }

    public int [] SelfBuff
    {
        get { return selfBuff; }
    }

    public int[] SelfDebuff
    {
        get { return selfDebuff; }
    }

    public int[] EnemyBuff
    {
        get { return enemyBuff; }
    }

    public int[] EnemyDebuff
    {
        get { return enemyDebuff; }
    }

    public float LoveMultiplier
    {
        get { return LoveHealMultiplier; }
    }
}


