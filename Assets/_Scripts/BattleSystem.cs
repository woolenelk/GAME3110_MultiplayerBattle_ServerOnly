using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public enum BattleState { 
    START,
    PLAYER1,
    PLAYER2,
    WIN,
    LOOSE
}


public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject playerCharacter;
    public GameObject enemyCharacter;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        CreatePlayers();
    }

    void CreatePlayers()
    {
        Instantiate(playerCharacter);
        Instantiate(enemyCharacter);
    }


    
}
