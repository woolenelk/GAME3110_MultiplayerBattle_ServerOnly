using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinLobbyScript : MonoBehaviour
{
    [SerializeField]
    JoinablePlayer joinableLobby;

    public void OnJoinLobbyPressed()
    {
        FindObjectOfType<NetworkClient>().JoinLobby(joinableLobby.AvailableLobby);
    }
}
