using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
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
        REQUEST_ALL_LOBBIES,
        START_GAME,
        MOVE_TAKEN,
        BATTLE_WON,
        PLAYER_INFO
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
    [System.Serializable]
    public class HostGameMsg:NetworkHeader
    {
        
        public NetworkObjects.NetworkPlayer player;
        public NetworkObjects.Lobby newLobby;
        public bool successful;

        public HostGameMsg()
        {
            cmd = Commands.HOST_GAME;
            newLobby = new NetworkObjects.Lobby();
            player = new NetworkObjects.NetworkPlayer();
            successful = false;
        }
    }
    [System.Serializable]
    public class JoinGameMsg : NetworkHeader 
    {
        public NetworkObjects.NetworkPlayer player;
        public NetworkObjects.Lobby joinLobby;
        public bool successful;

        public JoinGameMsg()
        {
            cmd = Commands.JOIN_GAME;
            joinLobby = new NetworkObjects.Lobby();
            player = new NetworkObjects.NetworkPlayer();
            successful = false;
        }
    }

    public class StartGameMsg : NetworkHeader
    {
        public NetworkObjects.Lobby LobbyToStart;
        public bool successful;

        public StartGameMsg()
        {
            cmd = Commands.START_GAME;
            LobbyToStart = new NetworkObjects.Lobby();
            successful = false;
        }
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
            Lobbies = new List<NetworkObjects.Lobby>();
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
    [System.Serializable]
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
    [System.Serializable]
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
    [System.Serializable]
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

    [System.Serializable]
    public class MoveTakenMsg : NetworkHeader
    {
        public NetworkObjects.Lobby Lobby;
        public int move;
        public MoveTakenMsg()
        {      // Constructor
            cmd = Commands.MOVE_TAKEN;
            Lobby = new NetworkObjects.Lobby();
            move = -1;
        }
    }

    [System.Serializable]
    public class BattleWinMsg : NetworkHeader
    {
        public NetworkObjects.Lobby Lobby;
        public bool won;
        public BattleWinMsg()
        {      // Constructor
            cmd = Commands.BATTLE_WON;
            Lobby = new NetworkObjects.Lobby();
            won = true;
        }
    }

    [System.Serializable]
    public class MyInfoMsg:NetworkHeader
    {
        public NetworkObjects.Item Player;
        public bool successful;

        public MyInfoMsg()
        {
            cmd = Commands.PLAYER_INFO;
            Player = new NetworkObjects.Item();
            successful = false;
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

        public Item()
        {
            UserID = null;
            Password = null;
            Wins = null;
            Loses = null;
        }
    }

    [System.Serializable]
    public class PlayerList
    {
        Item[] Items;
        public PlayerList ()
        {
            
        }
    }

    [System.Serializable]
    public class Lobby
    {
        public int lobbyID;
        public string Player1;
        public int player1addr;
        public string Player2;
        public int player2addr;
        public bool full;

        public Lobby()
        {
            lobbyID = -1;
            Player1 = "";
            player1addr = -1;
            Player2 = null;
            player2addr = -1;
            full = false;
        }
    }
}
