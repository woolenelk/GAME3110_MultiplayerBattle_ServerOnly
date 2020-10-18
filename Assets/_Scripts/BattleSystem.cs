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
    public TextMeshProUGUI[] abilitiesButtons;

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
        UpdateAbilityUI();

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
        enemyDetails.waifu = Waifus.waifuList[1];
        enemyDetails.waifuSprite.sprite = enemyDetails.waifu.characterImage;
        enemyDetails.Health = enemyDetails.waifu.HealthMax;

        
        dialogueText.text = "You face off against " + enemyDetails.waifu.CharacterName;
    }

    void UpdateAbilityUI()
    {
        for (int i = 0; i < 5; i++)
        {
            abilitiesButtons[i].text = playerDetails.waifu.MyAbilties.abilityList[i].AbilityName;
        }
        
    }

    IEnumerator Attack (int ability, WaifuDetails attacker, WaifuDetails defender )
    {
        bool defenderDefeated = false;
        bool attackerDefeated = false;
        Ability move = attacker.waifu.MyAbilties.abilityList[ability];
        dialogueText.text = attacker.waifu.CharacterName + " uses " + move.AbilityName+ "!";
        yield return new WaitForSeconds(1);
        defenderDefeated = defender.TakeDamage((int)(attacker.waifu.Attack * move.AttackMultipier * (1.0f+0.05f*attacker.buffs[(int)BUFF_ARRAY.ATTACK]) ) );
        attackerDefeated = attacker.Recoil((int)(move.CostHp));
       
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

        dialogueText.text = move.Description;
        yield return new WaitForSeconds(1);


        attacker.Rest((int)(attacker.waifu.Love * move.LoveMultiplier* (1.0f + 0.05f * attacker.buffs[(int)BUFF_ARRAY.LOVE])));

        UpdateCharactersUI();

        yield return new WaitForSeconds(2);

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
    void UpdateCharactersUI()
    {
        enemyDetailsUI.FillUI(enemyDetails);
        playerDetailsUI.FillUI(playerDetails);

    }


    void Player1Turn()
    {
        dialogueText.text = "What would you like your waifu to do?";
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

        yield return new WaitForSeconds(1.0f);

        int move = (int)UnityEngine.Random.Range(0, 5);

        StartCoroutine(Attack(move, enemyDetails, playerDetails));

        yield return new WaitForSeconds(1.0f);
    }

    public void OnButtonAttack1(AudioSource buttonSound)
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        buttonSound.Play();
        StartCoroutine(Attack(0, playerDetails, enemyDetails)) ;
        

    }

    public void OnButtonAttack2(AudioSource buttonSound)
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        buttonSound.Play();
        StartCoroutine(Attack(1, playerDetails, enemyDetails));
    }

    public void OnButtonAttack3(AudioSource buttonSound)
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        buttonSound.Play();
        StartCoroutine(Attack(2, playerDetails, enemyDetails));
        
    }

    public void OnButtonGuardUp(AudioSource buttonSound)
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        buttonSound.Play();
        StartCoroutine(Attack(3, playerDetails, enemyDetails));
        
    }

    public void OnButtonRest(AudioSource buttonSound)
    {
        if (state != BattleState.PLAYER1)
            return;
        state = BattleState.PROCESSING;
        buttonSound.Play();
        StartCoroutine(Attack(4, playerDetails, enemyDetails));
    }

    

}
