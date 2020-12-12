using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using NetworkObjects;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public string serverIP;
    public ushort serverPort;


    public List<NetworkObjects.NetworkPlayer> connectedPlayers;
    public string myId = "";
    public List<NetworkObjects.NetworkPlayer> droppedPlayers;

    void Start ()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(serverIP,serverPort);
        m_Connection = m_Driver.Connect(endpoint);

        DontDestroyOnLoad(gameObject);
    }
    
    void SendToServer(string message){
        var writer = m_Driver.BeginSend(m_Connection);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(message),Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    void OnConnect(){
        Debug.Log("We are now connected to the server");

        //// Example to send a handshake message:
        //HandshakeMsg m = new HandshakeMsg();
        //m.player.id = m_Connection.InternalId.ToString();
        //SendToServer(JsonUtility.ToJson(m));

        //StartCoroutine(SendRepeatedHandshake());
    }


    //IEnumerator SendRepeatedHandshake()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(2);
    //        Debug.Log("Sending Handshake");
    //        HandshakeMsg m = new HandshakeMsg();
    //        m.player.id = m_Connection.InternalId.ToString();
    //        SendToServer(JsonUtility.ToJson(m));

    //    }
    //}


   

    void OnData(DataStreamReader stream){
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                Debug.Log("Handshake message received!");
                //add our own id so we know who we are
                if (myId == "")
                {
                    myId = hsMsg.player.id;
                    Debug.Log("My id is:" + myId);
                }
                connectedPlayers.Add(hsMsg.player);

                    break;
                case Commands.PLAYER_UPDATE:
                    //not really receiveing player update messages as this is this client and not the server
                PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>(recMsg);
                Debug.Log("Player update message received!");
            break;
            case Commands.SERVER_UPDATE:
                ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>(recMsg);
                Debug.Log("Server update message received!");
                for (int i = 0; i < suMsg.players.Count; i++)
                {
                    foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
                    {
                        if (player.id == suMsg.players[i].id)//get the matching player from the server and out player list
                        {
                        ;
                        }
                    }

                }
                break;
            case Commands.NEWPLAYER_UPDATE:
                NewPlayerUpdateMsg npMsg = JsonUtility.FromJson<NewPlayerUpdateMsg>(recMsg);
                for (int i = 0; i < npMsg.players.Count; i++)
                {
                    //check if there are any new players that were added
                    bool playerFound = false;
                    foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
                    {
                        if (npMsg.players[i].id == player.id)
                        {
                            playerFound = true;
                            Debug.Log("already have the player");

                        }

                    }
                    if (!playerFound) // the player in the latest game state is new and we need to add them
                    {
                        connectedPlayers.Add(npMsg.players[i]);

                        Debug.Log("Added other player to conencted players");

                    }
                }
                break;
            case Commands.DROPPED_UPDATE:
                DroppedUpdateMsg dpMsg = JsonUtility.FromJson<DroppedUpdateMsg>(recMsg);
                foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
                {
                    if (player.id == dpMsg.player.id)
                    {
                        Debug.Log("found dropped player");
                        droppedPlayers.Add(player);
                    }
                }
                break;
            default:
            Debug.Log("Unrecognized message received!");
            break;
        }
    }

    void SpawnPlayers()
    {
        foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
        {
            
        }



    }

    void UpdatePlayers()
    {
        //update player position
        foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
        {
            
        }
    }

    void DestroyPlayers()
    {

        
        //clear the dropped players
        droppedPlayers.Clear();

    }

    //send our player data to the server
    void UpdatePosition()
    {
        
        foreach (NetworkObjects.NetworkPlayer player in connectedPlayers)
        {
            if (player.id == myId) //find our player
            {

                //send out player info to the server
                PlayerUpdateMsg m = new PlayerUpdateMsg();
                SendToServer(JsonUtility.ToJson(m));

            }
        }
    }

    void Disconnect(){
        m_Connection.Disconnect(m_Driver);
        m_Connection = default(NetworkConnection);
    }

    void OnDisconnect(){
        Debug.Log("Client got disconnected from server");
        m_Connection = default(NetworkConnection);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }   
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        cmd = m_Connection.PopEvent(m_Driver, out stream);
        while (cmd != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                OnConnect();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                OnDisconnect();
            }

            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }

        SpawnPlayers();
        UpdatePosition();
        UpdatePlayers();
        DestroyPlayers();

    }
}