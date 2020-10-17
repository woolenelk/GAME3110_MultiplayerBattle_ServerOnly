using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BattleState { 
    START,
    PLAYER1,
    PLAYER2,
    WIN,
    LOOSE,
    PROCESSING
}


public class BattleSystem : MonoBehaviour
{
    //knows the state of the current game
    public BattleState state;
    //which characters to instantiate
    public GameObject playerCharacter;
    public GameObject enemyCharacter;

    //details of our charatcer
    WaifuDetails playerDetails;
    WaifuDetails enemyDetails;

    public TextMeshProUGUI dialogueText;

    // the UI panels for the respective characters
    public DetailsUI playerDetailsUI;
    public DetailsUI enemyDetailsUI;


    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(CreatePlayers());
    }

    //a corutine to allow for us to have delays
    IEnumerator CreatePlayers()
    {
        //instantiate player and get their detials
        GameObject player = Instantiate(playerCharacter);
        playerDetails = player.GetComponent<WaifuDetails>();

        //nstantiate enemy and get their detials
        GameObject enemy = Instantiate(enemyCharacter);
        enemyDetails = enemy.GetComponent<WaifuDetails>();


        dialogueText.text = "You face off against " + enemyDetails.CharacterName;

        playerDetailsUI.FillUI(playerDetails);
        enemyDetailsUI.FillUI(enemyDetails);

        yield return new WaitForSeconds(3.0f);

        //currently have player 1 always start
        state = BattleState.PLAYER1;

        Player1Turn();

    }


    void Player1Turn()
    {
        dialogueText.text = "What would you like your waifu to do?";
    }

    IEnumerator Player1Attack1()
    {
        //Damage
        bool defeated = enemyDetails.TakeDamage(playerDetails.Attack);

        enemyDetailsUI.UpdateHP(enemyDetails.Health);
        playerDetailsUI.UpdateHP(playerDetails.Health);

        dialogueText.text = "The attack was successful";

        yield return new WaitForSeconds(1f);

        if (defeated)
        {
            state = BattleState.WIN;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYER2;
            StartCoroutine(EnemyTurn());
        }

    }


    void EndBattle()
    {
        if (state == BattleState.WIN)
        {
            dialogueText.text = "You won the battle!";

        }
        else if (state == BattleState.LOOSE)
        {
            dialogueText.text = "You lost the battle!";

        }
    }

    IEnumerator EnemyTurn()
    {
        //put AI HERE

        dialogueText.text = enemyDetails.CharacterName + " attacks!";

        yield return new WaitForSeconds(2f);

        bool defeated = playerDetails.TakeDamage(enemyDetails.Attack);
        playerDetailsUI.UpdateHP(playerDetails.Health);

        yield return new WaitForSeconds(2f);

        if (defeated)
        {
            state = BattleState.LOOSE;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYER1;
            Player1Turn();
        }


    }

    IEnumerator Player1Rest()
    {
        playerDetails.Rest(playerDetails.Love);
        playerDetailsUI.UpdateHP(playerDetails.Health);
        dialogueText.text = playerDetails.CharacterName + " has been healed by your love.";
        yield return new WaitForSeconds(1f);
        state = BattleState.PLAYER2;
        StartCoroutine(EnemyTurn());
    }

    public void OnButtonAttack1()
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        StartCoroutine(Player1Attack1());

    }

    public void OnButtonAttack2()
    {
        if (state != BattleState.PLAYER1)
            return;
    }

    public void OnButtonAttack3()
    {
        if (state != BattleState.PLAYER1)
            return;
    }

    public void OnButtonGuardUp()
    {
        if (state != BattleState.PLAYER1)
            return;
    }

    public void OnButtonRest()
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        StartCoroutine(Player1Rest());
    }

}
