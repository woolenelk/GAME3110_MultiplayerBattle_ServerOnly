using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NetworkMessages;
using UnityEngine.SceneManagement;

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

    public NetworkClient networkClient;

    public BattleState MyPlayerNum;
    bool MoveAvailble;


    // Start is called before the first frame update
    void Start()
    {
        MoveAvailble = true;
        networkClient = FindObjectOfType<NetworkClient>();
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

        //TODO: get the waifu from the network client
        if (networkClient.MyLobby.Player1 == networkClient.PlayerUserID)//if player 1
        {
            playerDetails.waifu = Waifus.waifuList[networkClient.MyPlayerCharacter];
            MyPlayerNum = BattleState.PLAYER1;
          
        }
        else
        {
            playerDetails.waifu = Waifus.waifuList[networkClient.MyPlayerCharacter];
            MyPlayerNum = BattleState.PLAYER2;
            
        }

        playerDetails.waifuSprite.sprite = playerDetails.waifu.characterImage;
        playerDetails.Health = playerDetails.waifu.HealthMax;
    }

    void CreateEnemy()
    {
        //nstantiate enemy and get their detials
        GameObject enemy = Instantiate(enemyCharacter);
        enemyDetails = enemy.GetComponent<WaifuDetails>();
        //enemyDetails.waifu = Waifus.waifuList[1];
        //if (networkClient.MyLobby.Player1 == networkClient.PlayerUserID)//if player 1
        //{
        enemyDetails.waifu = Waifus.waifuList[networkClient.EnemyPlayerCharacter];

        //}
        //else
        //{
        //    enemyDetails.waifu = Waifus.waifuList[networkClient.EnemyPlayerCharacter];
        //}
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

    IEnumerator Attack(int ability, WaifuDetails attacker, WaifuDetails defender)
    {
        bool defenderDefeated = false;
        bool attackerDefeated = false;
        Ability move = attacker.waifu.MyAbilties.abilityList[ability];
        dialogueText.text = attacker.waifu.CharacterName + " uses " + move.AbilityName + "!";
        yield return new WaitForSeconds(1);
        defenderDefeated = defender.TakeDamage((int)(attacker.waifu.Attack * move.AttackMultipier * (1.0f + 0.05f * attacker.buffs[(int)BUFF_ARRAY.ATTACK])));
        attackerDefeated = attacker.Recoil((int)(move.CostHp));

        // buffs and debuffs
        attacker.buffs[(int)BUFF_ARRAY.ATTACK] = move.SelfBuff[(int)BUFF_ARRAY.ATTACK];
        attacker.buffs[(int)BUFF_ARRAY.DEFENCE] = move.SelfBuff[(int)BUFF_ARRAY.DEFENCE];
        attacker.buffs[(int)BUFF_ARRAY.LOVE] = move.SelfBuff[(int)BUFF_ARRAY.LOVE];
        attacker.buffs[(int)BUFF_ARRAY.ATTACK] -= move.SelfDebuff[(int)BUFF_ARRAY.ATTACK];
        attacker.buffs[(int)BUFF_ARRAY.DEFENCE] -= move.SelfDebuff[(int)BUFF_ARRAY.DEFENCE];
        attacker.buffs[(int)BUFF_ARRAY.LOVE] -= move.SelfDebuff[(int)BUFF_ARRAY.LOVE];

        defender.buffs[(int)BUFF_ARRAY.ATTACK] = move.SelfBuff[(int)BUFF_ARRAY.ATTACK];
        defender.buffs[(int)BUFF_ARRAY.DEFENCE] = move.SelfBuff[(int)BUFF_ARRAY.DEFENCE];
        defender.buffs[(int)BUFF_ARRAY.LOVE] = move.SelfBuff[(int)BUFF_ARRAY.LOVE];
        defender.buffs[(int)BUFF_ARRAY.ATTACK] -= move.SelfDebuff[(int)BUFF_ARRAY.ATTACK];
        defender.buffs[(int)BUFF_ARRAY.DEFENCE] -= move.SelfDebuff[(int)BUFF_ARRAY.DEFENCE];
        defender.buffs[(int)BUFF_ARRAY.LOVE] -= move.SelfDebuff[(int)BUFF_ARRAY.LOVE];

        dialogueText.text = move.Description;
        yield return new WaitForSeconds(1);


        attacker.Rest((int)(attacker.waifu.Love * move.LoveMultiplier * (1.0f + 0.05f * attacker.buffs[(int)BUFF_ARRAY.LOVE])));

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

        Debug.Log("Move Made");
        if (state == BattleState.PLAYER2)
        {
            state = BattleState.PLAYER1;
            MoveAvailble = true;
            Player1Turn();
        }
        else if (state == BattleState.PLAYER1)
        {
            state = BattleState.PLAYER2;
            MoveAvailble = true;
            Player2Turn();
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
        if (MyPlayerNum == BattleState.PLAYER1)
        {
            dialogueText.text = "What would you like your waifu to do?";

        }
        else
        {
            dialogueText.text = "Waiting for opponent to make move";

        }

    }

    void EndBattle()
    {
        if (state == BattleState.WIN)
        {
            networkClient.PlayerWon();
            dialogueText.text = "You won the battle!";

        }
        else if (state == BattleState.LOOSE)
        {
            dialogueText.text = "You lost the battle!";

        }

        StartCoroutine(ReturnToLobbies());
    }

    IEnumerator ReturnToLobbies()
    {
        yield return new WaitForSeconds(3.0f);
        //reset network client values
        networkClient.MyLobby = new NetworkObjects.Lobby();
        SceneManager.LoadScene("Lobbies");
    }

    public void Player2Turn()
    {
        if (MyPlayerNum == BattleState.PLAYER2)
        {
            dialogueText.text = "What would you like your waifu to do?";

        }
        else
        {
            dialogueText.text = "Waiting for opponent to make move";

        }
    }
    public void EnemyAttack(int move)
    {
        Debug.Log("Enemy Uses ability: " + move);
        StartCoroutine(StartEnemyAttack(move));
    }
    IEnumerator StartEnemyAttack(int move)
    {
        dialogueText.text = enemyDetails.waifu.CharacterName + " attacks!";
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Attack(move, enemyDetails, playerDetails));
        yield return new WaitForSeconds(1.0f);
    }

    //IEnumerator EnemyTurn()
    //{
    //    //put AI HERE

    //    dialogueText.text = enemyDetails.waifu.CharacterName + " attacks!";

    //    yield return new WaitForSeconds(1.0f);

    //    int move = (int)UnityEngine.Random.Range(0, 5);

    //    StartCoroutine(Attack(move, enemyDetails, playerDetails));

    //    yield return new WaitForSeconds(1.0f);
    //}

    public void OnButtonAttack1(AudioSource buttonSound)
    {
        //player 1 turn and player 1
        if (MyPlayerNum == BattleState.PLAYER1 && MoveAvailble)
        {
            if (state != BattleState.PLAYER1)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(0);
            StartCoroutine(Attack(0, playerDetails, enemyDetails));
        }
        else if(MyPlayerNum == BattleState.PLAYER2 & MoveAvailble)//player 2 turn and player 2
        {
            if (state != BattleState.PLAYER2)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(0);
            StartCoroutine(Attack(0, playerDetails, enemyDetails));
        }

    }

    public void OnButtonAttack2(AudioSource buttonSound)
    {
        if (MyPlayerNum == BattleState.PLAYER1 && MoveAvailble)
        {
            if (state != BattleState.PLAYER1)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(1);
            StartCoroutine(Attack(1, playerDetails, enemyDetails));
        }
        else if (MyPlayerNum == BattleState.PLAYER2 && MoveAvailble)//player 2 turn and player 2
        {
            if (state != BattleState.PLAYER2)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(1);
            StartCoroutine(Attack(1, playerDetails, enemyDetails));
        }
    }

    public void OnButtonAttack3(AudioSource buttonSound)
    {
        if (MyPlayerNum == BattleState.PLAYER1 && MoveAvailble)
        {
            if (state != BattleState.PLAYER1)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(2);
            StartCoroutine(Attack(2, playerDetails, enemyDetails));
        }
        else if (MyPlayerNum == BattleState.PLAYER2 && MoveAvailble)//player 2 turn and player 2
        {
            if (state != BattleState.PLAYER2)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(2);
            StartCoroutine(Attack(2, playerDetails, enemyDetails));
        }
    }
    public void OnButtonGuardUp(AudioSource buttonSound)
    {
        if (MyPlayerNum == BattleState.PLAYER1 && MoveAvailble)
        {
            if (state != BattleState.PLAYER1)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(3);
            StartCoroutine(Attack(3, playerDetails, enemyDetails));
        }
        else if (MyPlayerNum == BattleState.PLAYER2 && MoveAvailble)//player 2 turn and player 2
        {
            if (state != BattleState.PLAYER2)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(3);
            StartCoroutine(Attack(3, playerDetails, enemyDetails));
        }
        
    }

    public void OnButtonRest(AudioSource buttonSound)
    {
        if (MyPlayerNum == BattleState.PLAYER1 && MoveAvailble)
        {
            if (state != BattleState.PLAYER1)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(4);
            StartCoroutine(Attack(4, playerDetails, enemyDetails));
        }
        else if (MyPlayerNum == BattleState.PLAYER2 && MoveAvailble)//player 2 turn and player 2
        {
            if (state != BattleState.PLAYER2)
                return;
            
            buttonSound.Play();
            MoveAvailble = false;
            networkClient.MakeMove(4);
            StartCoroutine(Attack(4, playerDetails, enemyDetails));
        }
    }

    

}
