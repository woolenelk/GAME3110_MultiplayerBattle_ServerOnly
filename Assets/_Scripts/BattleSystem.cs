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

    public WaifuMasterList Waifus;

    //details of our charatcer
    WaifuDetails playerDetails;
    WaifuDetails enemyDetails;

    public TextMeshProUGUI dialogueText;

    // the UI panels for the respective characters
    public DetailsUI playerDetailsUI;
    public DetailsUI enemyDetailsUI;

    public 


    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(CreatePlayers());
    }

    //a corutine to allow for us to have delays
    IEnumerator CreatePlayers()
    {

        CreatePlayer();
        CreateEnemy();
        UpdateCharactersUI();

        yield return new WaitForSeconds(3.0f);

        //currently have player 1 always start
        state = BattleState.PLAYER1;

        Player1Turn();

    }

    void CreatePlayer()
    {
        //instantiate player and get their detials
        GameObject player = Instantiate(playerCharacter);
        playerDetails = player.GetComponent<WaifuDetails>();
        //playerDetails.waifu = Waifus.waifuList[PlayerPrefs.GetInt("Player1")];
        playerDetails.waifu = Waifus.waifuList[0];
        playerDetails.waifuSprite.sprite = playerDetails.waifu.characterImage;
        playerDetails.Health = playerDetails.waifu.HealthMax;
    }

    void CreateEnemy()
    {
        //nstantiate enemy and get their detials
        GameObject enemy = Instantiate(enemyCharacter);
        enemyDetails = enemy.GetComponent<WaifuDetails>();
        enemyDetails.waifu = Waifus.waifuList[0];
        enemyDetails.waifuSprite.sprite = enemyDetails.waifu.characterImage;
        enemyDetails.Health = enemyDetails.waifu.HealthMax;

        
        dialogueText.text = "You face off against " + enemyDetails.waifu.CharacterName;
    }

    IEnumerator Attack (int ability, WaifuDetails attacker, WaifuDetails defender )
    {
        bool defenderDefeated = false;
        bool attackerDefeated = false;
        Ability move = attacker.waifu.MyAbilties.abilityList[ability];
        defenderDefeated = defender.TakeDamage((int)(attacker.waifu.Attack * move.AttackMultipier));
        attackerDefeated = attacker.TakeDamage((int)(move.CostHp));

        UpdateCharactersUI();

        // buffs and debuffs

        attacker.buffs[(int)BUFF_ARRAY.ATTACK]   = move.SelfBuff  [(int)BUFF_ARRAY.ATTACK];
        attacker.buffs[(int)BUFF_ARRAY.DEFENCE]  = move.SelfBuff  [(int)BUFF_ARRAY.DEFENCE];
        attacker.buffs[(int)BUFF_ARRAY.LOVE]     = move.SelfBuff  [(int)BUFF_ARRAY.LOVE];
        attacker.buffs[(int)BUFF_ARRAY.ATTACK]  -= move.SelfDebuff[(int)BUFF_ARRAY.ATTACK];
        attacker.buffs[(int)BUFF_ARRAY.DEFENCE] -= move.SelfDebuff[(int)BUFF_ARRAY.DEFENCE];
        attacker.buffs[(int)BUFF_ARRAY.LOVE]    -= move.SelfDebuff[(int)BUFF_ARRAY.LOVE];

        defender.buffs[(int)BUFF_ARRAY.ATTACK]   = move.SelfBuff  [(int)BUFF_ARRAY.ATTACK];
        defender.buffs[(int)BUFF_ARRAY.DEFENCE]  = move.SelfBuff  [(int)BUFF_ARRAY.DEFENCE];
        defender.buffs[(int)BUFF_ARRAY.LOVE]     = move.SelfBuff  [(int)BUFF_ARRAY.LOVE];
        defender.buffs[(int)BUFF_ARRAY.ATTACK]  -= move.SelfDebuff[(int)BUFF_ARRAY.ATTACK];
        defender.buffs[(int)BUFF_ARRAY.DEFENCE] -= move.SelfDebuff[(int)BUFF_ARRAY.DEFENCE];
        defender.buffs[(int)BUFF_ARRAY.LOVE]    -= move.SelfDebuff[(int)BUFF_ARRAY.LOVE];

        if (CheckPlayerWin())
        {
            state = BattleState.WIN;
            EndBattle();
        }
        if (CheckEnemyWin())
        {
            state = BattleState.LOOSE;
            EndBattle();
        }

        yield return new WaitForSeconds(2);

        if (state == BattleState.PLAYER2)
        {
            state = BattleState.PLAYER1;
            Player1Turn();
        }
        else if (state == BattleState.PROCESSING)
        {
            state = BattleState.PLAYER2;
            StartCoroutine(EnemyTurn());
        }
        
    }

    bool CheckPlayerWin()
    {
        /// return true if playerHasWon
        if (enemyDetails.Health <= 0)
            return true;
        return false;
    }

    bool CheckEnemyWin()
    {
        if (playerDetails.Health <= 0)
        {
            return true;
        }
        return false;
    }


    void Player1Turn()
    {
        dialogueText.text = "What would you like your waifu to do?";
    }

    IEnumerator Player1Attack1()
    {
        //Damage
        bool defeated = enemyDetails.TakeDamage(playerDetails.waifu.Attack);

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

        dialogueText.text = enemyDetails.waifu.CharacterName + " attacks!";

        yield return new WaitForSeconds(2f);

        bool defeated = playerDetails.TakeDamage(enemyDetails.waifu.Attack);
        UpdateCharactersUI();

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
        playerDetails.Rest(playerDetails.waifu.Love);
        playerDetailsUI.UpdateHP(playerDetails.Health);
        dialogueText.text = playerDetails.waifu.CharacterName + " has been healed by your love.";
        UpdateCharactersUI();
        yield return new WaitForSeconds(1f);
        state = BattleState.PLAYER2;
        StartCoroutine(EnemyTurn());
    }

    public void OnButtonAttack1()
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        StartCoroutine(Attack(0, playerDetails, enemyDetails)) ;

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

    void UpdateCharactersUI()
    {
        enemyDetailsUI.FillUI(enemyDetails);
        playerDetailsUI.FillUI(playerDetails);
    }

}
