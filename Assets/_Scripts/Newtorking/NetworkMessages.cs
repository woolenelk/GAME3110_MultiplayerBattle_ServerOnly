using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkMessages
{
    public enum Commands{
        PLAYER_UPDATE,
        SERVER_UPDATE,
        HANDSHAKE,
        PLAYER_INPUT,
        NEWPLAYER_UPDATE,
        PLAYER_LOGIN,
        PLAYER_REGISTER,
        DROPPED_UPDATE,
        HOST_GAME,
        JOIN_GAME,
        REQUEST_AVAILABLE_LOBBIES,
        REQUEST_ALL_LOBBIES
    }
    
    [System.Serializable]
    public class NetworkHeader{
        public Commands cmd;
    }

    [System.Serializable]
    public class HandshakeMsg:NetworkHeader{
        public NetworkObjects.NetworkPlayer player;
        public HandshakeMsg(){      // Constructor
            cmd = Commands.HANDSHAKE;
            player = new NetworkObjects.NetworkPlayer();
        }
    }

    public class HostGameMsg:NetworkHeader
    {
        public NetworkObjects.NetworkPlayer player;
        public bool successful = false;
    }

    public class JoinGameMsg : NetworkHeader 
    {
        public NetworkObjects.NetworkPlayer player;
        public NetworkObjects.Lobby joinLobby;
        public bool successful = false;
    }

    [System.Serializable]
    public class RequestAvailableLobbiesMsg : NetworkHeader
    {
        public RequestAvailableLobbiesMsg()
        {      // Constructor
            cmd = Commands.REQUEST_AVAILABLE_LOBBIES;
        }
    };

    [System.Serializable]
    public class AllAvailableLobbies : NetworkHeader
    {
        public List<NetworkObjects.Lobby> Lobbies;

        public AllAvailableLobbies()
        {      // Constructor
            cmd = Commands.REQUEST_ALL_LOBBIES;
        }
    };


    [System.Serializable]
    public class PlayerUpdateMsg:NetworkHeader{
        public NetworkObjects.NetworkPlayer player;
        public PlayerUpdateMsg(){      // Constructor
            cmd = Commands.PLAYER_UPDATE;
            player = new NetworkObjects.NetworkPlayer();
        }
    };

    public class PlayerLoginMsg : NetworkHeader
    {
        public string userID;
        public string password;
        public bool successful;
        public PlayerLoginMsg()
        {
            cmd = Commands.PLAYER_LOGIN;
            userID = "";
            password = "";
            successful = false;
        }
    }

    public class PlayerRegisterMsg : NetworkHeader
    {
        public string userID;
        public string password;
        public bool successful;
        public PlayerRegisterMsg()
        {
            cmd = Commands.PLAYER_REGISTER;
            userID = "";
            password = "";
            successful = false;
        }
    }

    public class PlayerInputMsg:NetworkHeader{
        public Input myInput;
        public PlayerInputMsg(){
            cmd = Commands.PLAYER_INPUT;
            myInput = new Input();
        }
    }
    [System.Serializable]
    public class  ServerUpdateMsg:NetworkHeader{
        public List<NetworkObjects.NetworkPlayer> players;
        public ServerUpdateMsg(){      // Constructor
            cmd = Commands.SERVER_UPDATE;
            players = new List<NetworkObjects.NetworkPlayer>();
        }
    }

    [System.Serializable]
    public class NewPlayerUpdateMsg : NetworkHeader
    {
        public List<NetworkObjects.NetworkPlayer> players;
        public NewPlayerUpdateMsg()
        {      // Constructor
            cmd = Commands.NEWPLAYER_UPDATE;
            players = new List<NetworkObjects.NetworkPlayer>();
        }
    }

    [System.Serializable]
    public class DroppedUpdateMsg : NetworkHeader
    {
        public NetworkObjects.NetworkPlayer player;
        public DroppedUpdateMsg()
        {      // Constructor
            cmd = Commands.DROPPED_UPDATE;
            player = new NetworkObjects.NetworkPlayer();
        }
    }
} 

namespace NetworkObjects
{
    [System.Serializable]
    public class NetworkObject{
        public string id;
    }
    [System.Serializable]
    public class NetworkPlayer : NetworkObject{

        public NetworkPlayer(){
        }
    }
    [System.Serializable]
    public class Item
    {
        public string UserID;
        public string Password;
        public string Wins;
        public string Loses;
    }

    public class PlayerList
    {
        Item[] Items;
    }

    [System.Serializable]
    public class Lobby
    {
        public int lobbyID;
        public string Player1;
        public string Player2;
        public bool full;
    }
}
