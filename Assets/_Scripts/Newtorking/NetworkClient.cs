using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;
public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public string serverIP;
    public ushort serverPort;

    string PlayerUserID = "";

    public string myId = "";

   

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

        StartCoroutine(SendRepeatedHandshake());
    }


    IEnumerator SendRepeatedHandshake()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            Debug.Log("Sending Handshake");
            HandshakeMsg m = new HandshakeMsg();
            m.player.id = m_Connection.InternalId.ToString();
            SendToServer(JsonUtility.ToJson(m));

        }
    }


    void OnData(DataStreamReader stream){
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
            case Commands.PLAYER_LOGIN:
                PlayerLoginMsg loginMsg = JsonUtility.FromJson<PlayerLoginMsg>(recMsg);
                // check if successful is true
                if (loginMsg.successful)
                {
                    Debug.Log("Successful Login");
                    PlayerUserID = loginMsg.userID;
                    SceneManager.LoadScene("Lobbies");
                }
                else
                {
                    Debug.Log("UNSuccessful Login");
                }
                break;
            case Commands.PLAYER_REGISTER:
                PlayerRegisterMsg registerMsg = JsonUtility.FromJson<PlayerRegisterMsg>(recMsg);
                if (registerMsg.successful)
                {
                    Debug.Log("Successful Register");
                    PlayerUserID = registerMsg.userID;
                    SceneManager.LoadScene("Lobbies");
                }
                else
                {
                    Debug.Log("UNSuccessful Register");
                }
                // check if successful is true
                break;
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                Debug.Log("Handshake message received!");
                //add our own id so we know who we are
                if (myId == "")
                {
                    myId = hsMsg.player.id;
                    Debug.Log("My id is:" + myId);
                }
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
                   
                }
                break;
            default:
            Debug.Log("Unrecognized message received!");
            break;
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

    

    }

    public void Login(string UserID, string Password)
    {
        PlayerLoginMsg loginmsg = new PlayerLoginMsg();
        loginmsg.userID = UserID;
        loginmsg.password = Password;
        SendToServer(JsonUtility.ToJson(loginmsg));
    }

    public void Register(string UserID, string Password)
    {
        PlayerRegisterMsg registsermsg = new PlayerRegisterMsg();
        registsermsg.userID = UserID;
        registsermsg.password = Password;
        SendToServer(JsonUtility.ToJson(registsermsg));
    }

    public void CreateLobby()
    {
        HostGameMsg hostMsg = new HostGameMsg();
        hostMsg.player.id = PlayerUserID;
    }


}