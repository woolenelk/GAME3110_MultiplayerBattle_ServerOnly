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

    public List<NetworkObjects.Lobby> AvailableLobbies;
    private int LOBBYCURRENTMAXID = 0;
    
    void Start ()
    {
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
            Debug.Log(test.UserID);
            //message
            if (password == test.Password)
            {
                PlayerLoginMsg m = new PlayerLoginMsg();
                m.userID = userID;
                m.successful = true;
                SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
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
            // register sucessful
        }
    }

    public void HostNewLobby(string UserID, int connection)
    {
        HostGameMsg m = new HostGameMsg();
        m.player.id = UserID;

        NetworkObjects.Lobby newLobby = new NetworkObjects.Lobby();
        if (newLobby != null)
            m.successful = true;
        newLobby.lobbyID = LOBBYCURRENTMAXID;
        newLobby.Player1 = UserID;
        AvailableLobbies.Add(newLobby);

        SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
        LOBBYCURRENTMAXID++;
    }

    public void JoinLobby(int LobbyID, string joiningUserID, int connection)
    {
        JoinGameMsg m = new JoinGameMsg();
        m.player.id = joiningUserID;
        
        for (int i = 0; i < AvailableLobbies.Count; i++)
        {
            if (AvailableLobbies[i].lobbyID == LobbyID)
            {
                if (AvailableLobbies[i].Player2 == null)
                {
                    AvailableLobbies[i].Player2 = joiningUserID;
                    m.successful = true;
                }
                i = AvailableLobbies.Count;
            }
        }
        SendToClient(JsonUtility.ToJson(m), m_Connections[connection]);
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
        Debug.Log(c.ToString());
        Debug.Log("Accepted a connection");
        Debug.Log("Added player:" + c.InternalId.ToString());
        // Example to send a handshake message:
        HandshakeMsg m = new HandshakeMsg();
        //add the new player to our list of connected players
        connectedPlayers.Add(m.player);
        SendToClient(JsonUtility.ToJson(m), c);

        //send the new player all the other players and the other players the new player
        foreach (NetworkConnection connection in m_Connections)
        {
            if (connection.IsCreated)
            {
                Debug.Log("Sending connected players to player:" + connection.InternalId.ToString());
                NewPlayerUpdateMsg n = new NewPlayerUpdateMsg();
                n.players = new List<NetworkObjects.NetworkPlayer>(connectedPlayers);
                SendToClient(JsonUtility.ToJson(n), connection);
            }
        }
    }

    void OnData(DataStreamReader stream, int i)  // i is the index in m_connection to get ip address
    {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
            case Commands.PLAYER_LOGIN:
                PlayerLoginMsg loginMsg = JsonUtility.FromJson<PlayerLoginMsg>(recMsg);
                StartCoroutine(SendLoginWebRequest(loginMsg.userID, loginMsg.password, i));
                break;
            case Commands.PLAYER_REGISTER:
                PlayerRegisterMsg registerMsg = JsonUtility.FromJson<PlayerRegisterMsg>(recMsg);
                StartCoroutine(SendRegisterWebRequest(registerMsg.userID, registerMsg.password, i));
                break;
            case Commands.HOST_GAME:
                HostGameMsg hostMsg = JsonUtility.FromJson<HostGameMsg>(recMsg);
                HostNewLobby(hostMsg.player.id, i);
                break;
            case Commands.JOIN_GAME:
                JoinGameMsg joinMsg = JsonUtility.FromJson<JoinGameMsg>(recMsg);
                JoinLobby(joinMsg.joinLobby.lobbyID, joinMsg.player.id, i);

                break;
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                Debug.Log("Handshake message received!");
                break;
            case Commands.PLAYER_UPDATE:
                PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>(recMsg);
                //Debug.Log("Player update message received!");
                //update the specific players data
                foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
                {
                    
                }
                break;
            case Commands.SERVER_UPDATE:
                ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>(recMsg);
                Debug.Log("Server update message received!");
                break;
            default:
                Debug.Log("SERVER ERROR: Unrecognized message received!");
                break;
        }
    }

    void OnDisconnect(int i){
        Debug.Log("Client disconnected from server");
        foreach (var player in connectedPlayers)
        {
            if(player.id == m_Connections[i].InternalId.ToString())
            {
                droppedPlayer = player;
            }
        }
        connectedPlayers.Remove(droppedPlayer);
        m_Connections[i] = default(NetworkConnection);

        //let all of the remaining clients know who dropped
        DroppedUpdateMsg d = new DroppedUpdateMsg();
        d.player = droppedPlayer;
        foreach (NetworkConnection connection in m_Connections)
        {
            if (connection.IsCreated)
            {
                SendToClient(JsonUtility.ToJson(d), connection);
            }
        }
        droppedPlayer = null;
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