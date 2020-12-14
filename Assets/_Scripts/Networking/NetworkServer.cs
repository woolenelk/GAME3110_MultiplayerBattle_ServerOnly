using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class NetworkServer : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort serverPort;
    private NativeList<NetworkConnection> m_Connections;

    public List<NetworkObjects.NetworkPlayer> connectedPlayers;
    public NetworkObjects.NetworkPlayer droppedPlayer;

   

    public Dictionary<string, NetworkConnection> d_Connections; // string is USERID, CONNECTION IS IP.ADDRESS/PORT
    public Dictionary<int,NetworkObjects.Lobby> AvailableLobbies = new Dictionary<int, NetworkObjects.Lobby>();

    private int LOBBYCURRENTMAXID = 0;
    
    void Start ()
    {
        



        d_Connections = new Dictionary<string, NetworkConnection>();
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = serverPort;
        if (m_Driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port " + serverPort);
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        //StartCoroutine(SendLoginWebRequest("kevin2", "test1", 0));
        //StartCoroutine(SendRegisterWebRequest("kevin3", "test1", 0));
        //StartCoroutine(SendHandshakeToAllClient());
        //StartCoroutine(SendUpdateToAllClient());
    }

    IEnumerator SendLoginWebRequest(string userID, string password, int connection)
    {
        string url = "https://pnz7w1hjm3.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentGetPlayer?UserID=" + userID;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            PlayerLoginMsg m = new PlayerLoginMsg();
            m.userID = userID;
            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
            // login unsuccessful
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            NetworkObjects.Item test = JsonUtility.FromJson<NetworkObjects.Item>(www.downloadHandler.text);
            //Debug.Log(test.UserID);
            //message
            if (password == test.Password)
            {
                PlayerLoginMsg m = new PlayerLoginMsg();
                m.userID = userID;
                m.successful = true;
                SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
                d_Connections[userID]= m_Connections[connection];
                // login successful
            }
            else
            {
                PlayerLoginMsg m = new PlayerLoginMsg();
                m.userID = userID;
                SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
                // login unsuccessful
            }
        }
    }

    IEnumerator PlayerInfoWebRequest(string userID, int connection)
    {
        string url = "https://pnz7w1hjm3.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentGetPlayer?UserID=" + userID;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log(" player info finished requesting from db ");
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(" player info error getting ");
            Debug.Log(www.error);
            MyInfoMsg m = new MyInfoMsg();
            m.Player.UserID = userID;
            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
            // register unsuccessful
        }
        else
        {
            Debug.Log(" player info successfully gotten. Sending to player ..... ");
            MyInfoMsg m = new MyInfoMsg();
            NetworkObjects.Item test = JsonUtility.FromJson<NetworkObjects.Item>(www.downloadHandler.text);
            m.Player = test;
            m.Player.UserID = test.UserID;
            m.Player.Wins = test.Wins;
            m.Player.Loses = test.Loses;
            m.successful = true;
            Debug.Log(JsonUtility.ToJson(m));
            Debug.Log(" ....... now. ");

            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
            // register sucessful
        }
    }
    IEnumerator SendRegisterWebRequest(string userID, string password, int connection)
    {
        string url = "https://mmhs1umqc0.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentAddPlayer?UserID=" + userID + "&Password=" + password;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            PlayerRegisterMsg m = new PlayerRegisterMsg();
            m.userID = userID;
            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
            // register unsuccessful
        }
        else
        {
            PlayerRegisterMsg m = new PlayerRegisterMsg();
            m.userID = userID;
            m.successful = true;
            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
            d_Connections[userID]= m_Connections[connection];
            // register sucessful
        }
    }

    void HostNewLobby(string UserID, int connection)
    {
        StartCoroutine(HostsNewLobby(UserID, connection));
    }

    IEnumerator HostsNewLobby(string UserID, int connection)
    {
        NetworkObjects.Item test;
        string url = "https://pnz7w1hjm3.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentGetPlayer?UserID=" + UserID;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log(" player info finished requesting from db ");
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(" player info error getting ");
            Debug.Log(www.error);
            // register unsuccessful
        }
        else
        {
            Debug.Log(" player info successfully gotten. Sending to player ..... ");
            test = JsonUtility.FromJson<NetworkObjects.Item>(www.downloadHandler.text);

            /////////////////////////////////////
            HostGameMsg m = new HostGameMsg();
            m.player.id = UserID;

            NetworkObjects.Lobby newLobby = new NetworkObjects.Lobby();
            if (newLobby != null)
            {
                m.successful = true;
                newLobby.lobbyID = LOBBYCURRENTMAXID;
                newLobby.Player1 = UserID;
                newLobby.player1addr = connection;
                newLobby.HostWins = int.Parse(test.Wins);
                m.newLobby = newLobby;
                AvailableLobbies[newLobby.lobbyID] = newLobby;
                LOBBYCURRENTMAXID++;
                Debug.Log("Lobby ID = " + newLobby.lobbyID);
                //Debug.Log(JsonUtility.ToJson(newLobby));
                //Debug.Log(JsonUtility.ToJson(AvailableLobbies[0]));
            }
            else
            {
                Debug.Log("Host Lobby Failed");
            }
            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
        }
    }

    public void JoinLobby(int LobbyID, string joiningUserID, int connection)
    {
        StartCoroutine(JoinsLobby(LobbyID, joiningUserID, connection));
    }

    IEnumerator JoinsLobby(int LobbyID, string joiningUserID, int connection)
    {

        NetworkObjects.Item test;
        string url = "https://pnz7w1hjm3.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentGetPlayer?UserID=" + UserID;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log(" player info finished requesting from db ");
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(" player info error getting ");
            Debug.Log(www.error);
            // register unsuccessful
        }
        else
        {
            Debug.Log(" player info successfully gotten. Sending to player ..... ");
            test = JsonUtility.FromJson<NetworkObjects.Item>(www.downloadHandler.text);

            //////////////////////////////////
            JoinGameMsg m = new JoinGameMsg();
            m.player.id = joiningUserID;

            foreach (var Lobby in AvailableLobbies)
            {
                if (Lobby.Key == LobbyID)
                {
                    if (Lobby.Value.Player2 == null)
                    {
                        Lobby.Value.Player2 = joiningUserID;
                        Lobby.Value.player2addr = connection;
                        Lobby.Value.full = true;
                        Lobby.Value.Player2Wins = int.Parse(test.Wins);
                        m.joinLobby = Lobby.Value;
                        m.successful = true;
                    }
                    break;
                }
            }
            SendToClient(JsonUtility.ToJson(m), m_Connections[m.joinLobby.player1addr]);
            SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
        }
    }

    void SendToClient(string message, NetworkConnection c)
    {
        var writer = m_Driver.BeginSend(NetworkPipeline.Null, c);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }
    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    void OnConnect(NetworkConnection c){
        m_Connections.Add(c);

        //d_Connections.Add(c.InternalId.ToString(), c);
        Debug.Log(c.ToString());
        Debug.Log(c.InternalId.ToString());
        Debug.Log("Accepted a connection");
        Debug.Log("Added player:" + c.InternalId.ToString());
        // Example to send a handshake message:
        HandshakeMsg m = new HandshakeMsg();
        //m.InternalServerID = c.InternalId.ToString();
        //add the new player to our list of connected players
        //connectedPlayers.Add(m.player);
        SendToClient(JsonUtility.ToJson(m), c);

        ////send the new player all the other players and the other players the new player
        //foreach (NetworkConnection connection in m_Connections)
        //{
        //    if (connection.IsCreated)
        //    {
        //        Debug.Log("Sending connected players to player:" + connection.InternalId.ToString());
        //        NewPlayerUpdateMsg n = new NewPlayerUpdateMsg();
        //        n.players = new List<NetworkObjects.NetworkPlayer>(connectedPlayers);
        //        SendToClient(JsonUtility.ToJson(n), connection);
        //    }
        //}
    }

    void OnData(DataStreamReader stream, int i)  // i is the index in m_connection to get ip address
    {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
            case Commands.PLAYER_LOGIN:
                Debug.Log("Login request received");
                PlayerLoginMsg loginMsg = JsonUtility.FromJson<PlayerLoginMsg>(recMsg);
                StartCoroutine(SendLoginWebRequest(loginMsg.userID, loginMsg.password, i));
                break;
            case Commands.PLAYER_REGISTER:
                Debug.Log("Registration request received");
                PlayerRegisterMsg registerMsg = JsonUtility.FromJson<PlayerRegisterMsg>(recMsg);
                StartCoroutine(SendRegisterWebRequest(registerMsg.userID, registerMsg.password, i));
                break;
            case Commands.HOST_GAME:
                Debug.Log("HOSTED GAME received");
                HostGameMsg hostMsg = JsonUtility.FromJson<HostGameMsg>(recMsg);
                HostNewLobby(hostMsg.player.id, i);
                break;
            case Commands.JOIN_GAME:
                Debug.Log("JOINED GAME received");
                JoinGameMsg joinMsg = JsonUtility.FromJson<JoinGameMsg>(recMsg);
                JoinLobby(joinMsg.joinLobby.lobbyID, joinMsg.player.id, i);
                
                break;
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                SendToClient(JsonUtility.ToJson(hsMsg), m_Connections[i]);
                //Debug.Log("Handshake message received!");
                break;
            case Commands.PLAYER_UPDATE:
                PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>(recMsg);
                //Debug.Log("Player update message received!");
                //update the specific players data
                foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
                {
                    
                }
                break;
            case Commands.START_GAME:
                StartGameMsg startMsg = JsonUtility.FromJson<StartGameMsg>(recMsg);
                startMsg.successful = true;
                startMsg.Player1Char = UnityEngine.Random.Range(0,4);
                int player2 = UnityEngine.Random.Range(0, 4);
                while(startMsg.Player1Char == player2)
                {
                    player2 = UnityEngine.Random.Range(0, 4);
                }
                startMsg.Player2Char = player2;
                Debug.Log("Start Message Sucess:");
                Debug.Log(startMsg.successful ? "Sucess": "Fail");
                Debug.Log(startMsg);
                SendToClient(JsonUtility.ToJson(startMsg), m_Connections[i]);
                SendToClient(JsonUtility.ToJson(startMsg), m_Connections[startMsg.LobbyToStart.player2addr]);
                
                break;
            case Commands.SERVER_UPDATE:
                ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>(recMsg);
                Debug.Log("Server update message received!");
                break;
            case Commands.REQUEST_AVAILABLE_LOBBIES:
                Debug.Log("Received request for lobbies");
                AllAvailableLobbies n = new AllAvailableLobbies();
                foreach (KeyValuePair<int, NetworkObjects.Lobby> Lobby in AvailableLobbies)
                {
                    if (!Lobby.Value.full)
                        n.Lobbies.Add(Lobby.Value);
                }
                //n.Lobbies = AvailableLobbies;
                //Debug.Log(JsonUtility.ToJson(AvailableLobbies[0]));
                //Debug.Log(JsonUtility.ToJson(n.Lobbies[0]));
                SendToClient(JsonUtility.ToJson(n), m_Connections[i]);
                break;
            case Commands.MOVE_TAKEN:
                MoveTakenMsg moveMsg = JsonUtility.FromJson<MoveTakenMsg>(recMsg);
                Debug.Log("Received Move from client");
                if (moveMsg.Lobby.player1addr == i)//if player 1 made the move
                {
                    SendToClient(JsonUtility.ToJson(moveMsg), m_Connections[moveMsg.Lobby.player2addr]);
                }
                else
                {
                    SendToClient(JsonUtility.ToJson(moveMsg), m_Connections[moveMsg.Lobby.player1addr]);
                }
                break;
            case Commands.PLAYER_INFO:
                MyInfoMsg infoMsg = JsonUtility.FromJson<MyInfoMsg>(recMsg);
                Debug.Log("Received Player Info from " + infoMsg.Player.UserID);
                StartCoroutine(PlayerInfoWebRequest(infoMsg.Player.UserID, i));
                break;
            case Commands.BATTLE_WON:
                BattleWinMsg winMsg = JsonUtility.FromJson<BattleWinMsg>(recMsg);
                Debug.Log("Received Move from client");
                if (winMsg.Lobby.player1addr == i)//if player 1 won
                {
                    StartCoroutine(SendWinAndLossWebRequest(winMsg.Lobby.Player1, winMsg.Lobby.Player2));
                        //update player 1 win and player 2 loss
                }
                else
                {
                    StartCoroutine(SendWinAndLossWebRequest(winMsg.Lobby.Player2, winMsg.Lobby.Player1));
                    //update player 2 win and player 1 loss
                }
                //remove the lobby from the available lobbies list
                AvailableLobbies.Remove(winMsg.Lobby.lobbyID);
                break;
            default:
                Debug.Log("SERVER ERROR: Unrecognized message received!");
                break;
        }
    }

    IEnumerator SendWinAndLossWebRequest(string Winner, string Loser)
    {
        string winurl = "https://6of6hcn9f8.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentPlayerWon?UserID=" + Winner;
        string lossurl = "https://k03zoudx96.execute-api.us-east-2.amazonaws.com/default/FinalAssignmentPlayerLost?UserID=" + Loser;

        UnityWebRequest www = UnityWebRequest.Get(winurl);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        UnityWebRequest www2 = UnityWebRequest.Get(lossurl);
        www2.SetRequestHeader("Content-Type", "application/json");
        yield return www2.SendWebRequest();
    }
    void OnDisconnect(int i){
        Debug.Log("Client disconnected from server");

        SendDroppedMessageToRemainingLobby(i);

        foreach (var player in connectedPlayers)
        {
            if(player.id == m_Connections[i].InternalId.ToString())
            {
                droppedPlayer = player;
            }
        }
        connectedPlayers.Remove(droppedPlayer);
        m_Connections[i] = default(NetworkConnection);


    }

    void SendDroppedMessageToRemainingLobby(int playerAddressThatDCd)
    {
        string playerName = ""; 
        foreach (var keyValue in d_Connections)
        {
            if (keyValue.Value == m_Connections[playerAddressThatDCd])
            {
                playerName = keyValue.Key;
            }
        }

        

        int closingLobby = -1;
        foreach (var keyValue in AvailableLobbies)
        {
            if(keyValue.Value.Player1 == playerName) //player 1 dc'd
            {
                closingLobby = keyValue.Key;
                if(keyValue.Value.full) //if the lobby is full
                {
                    //update win loss
                    StartCoroutine(SendWinAndLossWebRequest(keyValue.Value.Player2, keyValue.Value.Player1));
                    //send dc message to other player
                    LobbyDisconnectedMsg dcMSG = new LobbyDisconnectedMsg();
                    SendToClient(JsonUtility.ToJson(dcMSG), d_Connections[keyValue.Value.Player2]);

                }
            }
            else if(keyValue.Value.Player2 == playerName) //player2 dc
            {
                closingLobby = keyValue.Key;
                if (keyValue.Value.full) //if the lobby is full
                {
                    //update win loss
                    StartCoroutine(SendWinAndLossWebRequest(keyValue.Value.Player1, keyValue.Value.Player2));
                    //send dc message to other player
                    LobbyDisconnectedMsg dcMSG = new LobbyDisconnectedMsg();
                    SendToClient(JsonUtility.ToJson(dcMSG), d_Connections[keyValue.Value.Player1]);
                }
            }
        }

        AvailableLobbies.Remove(closingLobby);

        d_Connections.Remove(playerName);


    }


    void Update ()
    {
        m_Driver.ScheduleUpdate().Complete();

        // CleanUpConnections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {

                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // AcceptNewConnections
        NetworkConnection c = m_Driver.Accept();
        while (c  != default(NetworkConnection))
        {            
            OnConnect(c);

            // Check if there is another new connection
            c = m_Driver.Accept();
        }


        // Read Incoming Messages
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            Assert.IsTrue(m_Connections[i].IsCreated);
            
            NetworkEvent.Type cmd;
            cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream);
            while (cmd != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream, i);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    OnDisconnect(i);
                }

                cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream);
            }
        }
    }
}