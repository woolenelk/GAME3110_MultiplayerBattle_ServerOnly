using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLobbyButtonBehaviour : MonoBehaviour
{
    public void OnCreateLobbyButtonPressed()
    {

        NetworkClient networkManager = FindObjectOfType<NetworkClient>();
        networkManager.CreateLobby();
    }
}

