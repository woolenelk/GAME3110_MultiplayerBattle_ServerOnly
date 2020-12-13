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
    public NetworkObjects.Lobby MyLobby;
    public string PlayerUserID = "";
    public NetworkObjects.Item Player;
    //public string myServerId = "-1";

   

    void Start ()
    {
        Player = new NetworkObjects.Item();
        MyLobby = new NetworkObjects.Lobby();
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(serverIP, serverPort);
        m_Connection = m_Driver.Connect(endpoint);

        DontDestroyOnLoad(gameObject);
    }

    
    public void SendToServer(string message){
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
            yield return new WaitForSeconds(0.5f);
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
                    //RequestPlayerInfo();
                    SceneManager.LoadScene("Lobbies");
                }
                else
                {
                    Debug.Log("UNSuccessful Login");
                    FindObjectOfType<LoginButtonBehaviour>().DisplayError();
                }
                break;
            case Commands.PLAYER_REGISTER:
                PlayerRegisterMsg registerMsg = JsonUtility.FromJson<PlayerRegisterMsg>(recMsg);
                if (registerMsg.successful)
                {
                    Debug.Log("Successful Register");
                    PlayerUserID = registerMsg.userID;
                    //RequestPlayerInfo();
                    SceneManager.LoadScene("Lobbies");
                }
                else
                {
                    Debug.Log("UNSuccessful Register");
                    FindObjectOfType<RegisterButtonBehaviour>().DisplayError();
                }
                // check if successful is true
                break;
            case Commands.HOST_GAME:
                HostGameMsg hostmsg = JsonUtility.FromJson<HostGameMsg>(recMsg);
                if (hostmsg.successful && (hostmsg.newLobby.Player1 == PlayerUserID || hostmsg.newLobby.Player2 == PlayerUserID))
                {
                    MyLobby = hostmsg.newLobby;
                    SceneManager.LoadScene("Lobby");
                    // success move to the lobby scene
                }
                else
                {
                    // display error
                }
                break;
            case Commands.JOIN_GAME:
                JoinGameMsg joinmsg = JsonUtility.FromJson<JoinGameMsg>(recMsg);
                if (joinmsg.successful && (joinmsg.joinLobby.Player1 == PlayerUserID || joinmsg.joinLobby.Player2 == PlayerUserID))
                {
                    MyLobby = joinmsg.joinLobby;
                    if (joinmsg.joinLobby.Player1 == PlayerUserID)
                    {
                        FindObjectOfType<LobbyHandler>().UpdateLobby();
                    }
                    else
                    {
                        SceneManager.LoadScene("Lobby");
                    }
                    
                    // success move to the lobby scene
                }
                else
                {
                    // display error
                }
                break;
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);

                Debug.Log("Handshake message received!");
                //myServerId = hsMsg.InternalServerID;
                //Debug.Log("My id is:" + myServerId);
                ////add our own id so we know who we are
                //if (myId == "")
                //{
                //    myId = hsMsg.player.id;
                //    Debug.Log("My id is:" + myId);
                //}
                break;
            case Commands.START_GAME:
                StartGameMsg startMsg = JsonUtility.FromJson<StartGameMsg>(recMsg);
                if (startMsg.successful)
                {
                    EnterPlay();
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
            case Commands.REQUEST_ALL_LOBBIES:
                AllAvailableLobbies alMsg = JsonUtility.FromJson<AllAvailableLobbies>(recMsg);
                Debug.Log("Server update message received!");
                for (int i = 0; i < alMsg.Lobbies.Count; i ++)
                {
                    Debug.Log(JsonUtility.ToJson(alMsg.Lobbies[i]));
                }
                ScrollFiller scrollFiller = FindObjectOfType<ScrollFiller>();
                scrollFiller.ClearLobbies();
                foreach (var lobby in alMsg.Lobbies)
                {
                    //if (!lobby.full)
                    scrollFiller.GenerateItem(lobby);
                }
                break;
            case Commands.MOVE_TAKEN:
                MoveTakenMsg moveMsg = JsonUtility.FromJson<MoveTakenMsg>(recMsg);
                Debug.Log("Received move from player");
                FindObjectOfType<BattleSystem>().EnemyAttack(moveMsg.move);
                
                break;
            case Commands.PLAYER_INFO:
                MyInfoMsg infoMsg = JsonUtility.FromJson<MyInfoMsg>(recMsg);
                Debug.Log("Received info about player");
                Debug.Log( "RAW MESSAGE PLAYER INFO "+  recMsg);
                Player = infoMsg.Player;
                break;
            case Commands.LOBBY_DISCONNECTED:
                Debug.Log("Other player disconnected form lobby");
                OnOtherPlayerDisconnected();
                break;
            default:
                Debug.Log("Unrecognized message received!");
                Debug.Log(recMsg);
                break;
        }
    }

   
    public void RequestNewLobbies()
    {
        RequestAvailableLobbiesMsg m = new RequestAvailableLobbiesMsg();
        SendToServer(JsonUtility.ToJson(m));
    }

    void Disconnect(){
        m_Connection.Disconnect(m_Driver);
        m_Connection = default(NetworkConnection);
    }

    void OnDisconnect(){
        Debug.Log("Client got disconnected from server");
        m_Connection = default(NetworkConnection);
        SceneManager.LoadScene("Start Menu");
        Destroy(gameObject);
    }
    void OnOtherPlayerDisconnected()
    {
        SceneManager.LoadScene("Lobbies");
        MyLobby = null;
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
        Debug.Log("Client Create Lobby Called");
        HostGameMsg hostMsg = new HostGameMsg();
        hostMsg.player.id = PlayerUserID;
        SendToServer(JsonUtility.ToJson(hostMsg));
    }

    public void SendServerStartSignal()
    {
        Debug.Log("Sending Start Game to server");
        StartGameMsg m = new StartGameMsg();
        m.LobbyToStart = MyLobby;
        SendToServer(JsonUtility.ToJson(m));
    }

    public void EnterPlay()
    {
        Debug.Log("Loading Play Scene");
        SceneManager.LoadScene("Play");
    }

    public void JoinLobby(NetworkObjects.Lobby joiningLobby)
    {
        Debug.Log("Sending join game message");
        Debug.Log(joiningLobby);
        JoinGameMsg m = new JoinGameMsg();
        m.player.id = PlayerUserID;
        m.joinLobby = joiningLobby;
        SendToServer(JsonUtility.ToJson(m));
    }

    public bool isHostingPlayer()
    {
        return (MyLobby.Player1 == PlayerUserID);
    }

    public void MakeMove(int MoveNum)
    {
        MoveTakenMsg m = new MoveTakenMsg();
        m.move = MoveNum;
        m.Lobby = MyLobby;
        SendToServer(JsonUtility.ToJson(m));
    }

    public void PlayerWon()
    {
        BattleWinMsg m = new BattleWinMsg();
        m.Lobby = MyLobby;
        SendToServer(JsonUtility.ToJson(m));
    }

    public void RequestPlayerInfo()
    {
        
        MyInfoMsg infomsg = new MyInfoMsg();
        infomsg.Player.UserID = PlayerUserID;
        Debug.Log("Requesting Player Info");
        Debug.Log(JsonUtility.ToJson(infomsg));
        SendToServer(JsonUtility.ToJson(infomsg));
    }
}