using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}


