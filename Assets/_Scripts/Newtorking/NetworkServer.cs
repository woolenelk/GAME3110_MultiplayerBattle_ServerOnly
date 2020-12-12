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

public class NetworkServer : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort serverPort;
    private NativeList<NetworkConnection> m_Connections;

    public List<NetworkObjects.NetworkPlayer> connectedPlayers;
    public NetworkObjects.NetworkPlayer droppedPlayer;

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

        //StartCoroutine(SendHandshakeToAllClient());
        //StartCoroutine(SendUpdateToAllClient());
    }

    //IEnumerator SendHandshakeToAllClient()
    //{
    //    while(true)
    //    {
    //        for (int i = 0; i < m_Connections.Length; i++)
    //        {
    //            if (!m_Connections[i].IsCreated)
    //                continue;

    //            HandshakeMsg m = new HandshakeMsg();
    //            m.player.id = m_Connections[i].InternalId.ToString();
    //            SendToClient(JsonUtility.ToJson(m), m_Connections[i]); 

    //        }
    //        yield return new WaitForSeconds(2);
    //    }
    //}

    //IEnumerator SendUpdateToAllClient()
    //{
    //    //send the update to all players connected to the server
    //    while (true)
    //    {
    //        for (int i = 0; i < m_Connections.Length; i++)
    //        {
    //            if (!m_Connections[i].IsCreated)
    //                continue;
    //            ServerUpdateMsg m = new ServerUpdateMsg();
    //            m.players = connectedPlayers;
    //            SendToClient(JsonUtility.ToJson(m), m_Connections[i]);

    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}



    void SendToClient(string message, NetworkConnection c){
        var writer = m_Driver.BeginSend(NetworkPipeline.Null, c);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(message),Allocator.Temp);
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

    void OnData(DataStreamReader stream, int i){
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
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