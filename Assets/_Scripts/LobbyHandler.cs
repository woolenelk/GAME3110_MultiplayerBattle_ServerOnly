using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;

    NetworkObjects.Lobby ActiveLobby;

    // Start is called before the first frame update
    void Start()
    {
        Player2.SetActive(false);
        UpdateLobby();
        //subscribe to player joined lobby event received from the client network manager
        //subscribe to OnPlayer2JoinedLobbyHandler
    }

    public void UpdateLobby()
    {
        ActiveLobby = FindObjectOfType<NetworkClient>().MyLobby;
        if (ActiveLobby.Player2 != "")
        {
            Player2.SetActive(true);
            Player2.GetComponent<Image>().color = Color.red;
        }
    }
}
