using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLobbyButtonBehaviour : MonoBehaviour
{
    public void OnCreateLobbyButtonPressed()
    {
        Debug.Log("Create a Lobby Pressed");
        FindObjectOfType<NetworkClient>().CreateLobby();
    }
}

