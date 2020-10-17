using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaifuList", menuName = "ScriptableObjects/WaifuList", order = 4)]
public class WaifuMasterList : ScriptableObject
{
    public WaifuCreator[] waifuList;
   
}
