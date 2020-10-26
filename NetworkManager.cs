using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine.Networking;
public class NetworkManager : MonoBehaviour
{
    /*
     * SINGLTON CLASS THAT IS ACCESSABLE TO ALL THE SCENES IN THE GAME, 
     * MANAGES ALL THE NETWORKING ASPECTS OF THE GAME.
    */
    private static NetworkManager _instance;

    public static NetworkManager Instance { get { return _instance; } }

    public string username = null;
    public string password = null;
    public int coins = 0;
    public int xp = 0;
    public Sprite[] sprites = new Sprite[41];

    //Socket Variables
    public TcpClient socketConnection;
    private Thread clientReceiveThread;
    private bool isConnected = false;
    private float reconnect_time = 0.0f;
    private bool self_sync = false;

    //Events
    public delegate void GameServer(JSONObject js);
    public delegate void Server(string s);

    public static event Server OnPeerRequest;
    public static event Server OnWin;
    public static event Server OnLoose;
    public static event Server OnDraw;
    public static event Server OnPeerDeclined;

    public static event GameServer OnBoardData;
    public static event GameServer OnGameServerCredentialsRecieved;

    public static event Server OnServerConnected;
    public static event Server OnServerDisconnect;
    public static event Server OnListenDataEnded;
    public static event Server OnListenDataStarted;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        if(PlayerPrefs.HasKey("username"))
        {
            username = PlayerPrefs.GetString("username");
            password = PlayerPrefs.GetString("password");
            coins = PlayerPrefs.GetInt("coins");
            xp = PlayerPrefs.GetInt("xp");
        }
        else
        {

        }

        //connect("_CONEC_");
    }

    public void connect(string flag)
    {
        ConnectToServer("server.wordsfight.com", 4444, flag, "");
    }

    private void Update()
    {
        if(socketConnection != null)
        {
            if (socketConnection.Connected)
            {
                if (OnServerConnected != null)
                {
                    isConnected = true;
                    OnServerConnected("Open : " + socketConnection.Client.RemoteEndPoint);
                }

                if (!clientReceiveThread.IsAlive)
                {
                    clientReceiveThread.Abort();
                    clientReceiveThread = new Thread(() => ListenForData(null, 0, null, null));
                    clientReceiveThread.IsBackground = true;
                    clientReceiveThread.Start();
                }
            }
            else
            {
                OnServerDisconnect?.Invoke("Close");
            }
            /*
            else if(socketConnection.Connected == false)
            {

                OnServerDisconnect("Close");
                isConnected = false;
                socketConnection.Close();

                //Try to reconnect after 5 seconds of connection loss
                while (reconnect_time > 5)
                {

                    reconnect_time = 0f;
                    connect("_CONEC_");
                }
                reconnect_time += Time.deltaTime;
            }
            else
            {
                reconnect_time = 0f;
            }

            */
        }
    }

    public bool StopConnection()
    {
        if (socketConnection != null)
        { 
            socketConnection.Close();
        }
        return true;
    }

    public void Test()
    {
        while(true)
        {
            Debug.Log("GO");
            Debug.Log("GO");
            goto ENDOFLOOPS;
        }
        ENDOFLOOPS:
        {
            Debug.Log("ENDED LOOP");
        }
    }

    public void ConnectToServer(string ip, int port, string flag, string peer)
    {
        socketConnection = new TcpClient(ip, port);
        Byte[] bytes = new Byte[1024];

        JSONObject js = new JSONObject();

        js.AddField("UID", username);

        try
        {
            if (PlayerPrefs.HasKey("picID"))
                    js.AddField("AVATAR_ID", PlayerPrefs.GetInt("picID").ToString());
            else
                    js.AddField("AVATAR_ID", (00).ToString());
        }
        catch(Exception e)
        {
            Debug.LogError("ConnectToServer UID Error : " + e.Message);
        }
        

        js.AddField("MODE", flag);
        js.AddField("PEER", peer);

        if (socketConnection == null || socketConnection.Connected == false)
        { return; }
        try
        {
            isConnected = socketConnection.Connected;
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = js.ToString();
                byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("CONNECT TO SERVER -->  (" + ip + ", " + port.ToString() + ")");
            }
        }
        catch (SocketException socketException)
        { Debug.Log("Socket exception: " + socketException); }

        try
        {
            
            clientReceiveThread = new Thread(() => ListenForData(ip, port, flag, peer));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    
    public void ListenForData(string ip, int port, string flag, string peer)
    {
        Debug.Log("Listening Started....");

        Byte[] bytes = new Byte[1024];
        try
        {
            while (socketConnection.Connected)
            {
                try
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            string serverMessage = Encoding.UTF8.GetString(incommingData);

                            if (serverMessage.Contains("_PEER_REQ_"))
                            {
                                OnPeerRequest?.Invoke(serverMessage.Substring(10));
                            }
                            else if (serverMessage == "#PEER DECLINED")
                            {
                                OnPeerDeclined?.Invoke(serverMessage);
                            }
                            else if (serverMessage == "#WON")
                            {
                                OnWin?.Invoke(serverMessage);
                            }
                            else if (serverMessage == "#DRAW")
                            {
                                OnDraw?.Invoke(serverMessage);
                            }
                            else if (serverMessage == "#LOOSE")
                            {
                                OnLoose?.Invoke(serverMessage);
                            }
                            else if (serverMessage == " ")
                            {
                                Debug.Log(serverMessage + " --> " + socketConnection.Client.RemoteEndPoint);
                            }
                            else
                            {
                                try
                                {
                                    JSONObject js = new JSONObject(serverMessage);
                                    Debug.Log("JSON --- > " + js.ToString());

                                    if (js.HasField("IP"))
                                    {
                                        //Connect to GameServer
                                        OnGameServerCredentialsRecieved?.Invoke(js);
                                    }
                                    else if (js.HasField("_BOARD_"))
                                    {
                                        OnBoardData?.Invoke(js);
                                    }
                                }
                                catch (Exception e)
                                { Debug.Log("Exception: " + e); }
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Debug.Log("NETWORK STREAM READ : Exception");
                }
                
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }

        if (OnListenDataEnded != null)
            OnListenDataEnded("Listening Ended");
    }
    
    public void SendMessageToServer(string msg)
    {
        if (socketConnection == null || socketConnection.Connected == false)
        { return; }
        try
        {   
            isConnected = socketConnection.Connected;
            NetworkStream stream = socketConnection.GetStream();

            if (stream.CanWrite)
            {
                string clientMessage = msg;
                byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        { Debug.Log("Socket exception: " + socketException); }
    }

    public void OnDestroy()
    {
        try
        {
            if (socketConnection.Connected)
                socketConnection.Close();
        }
        catch(Exception e) 
        {
            Debug.Log("ON-DESTROY and Socket EXCEPTION");
        }
    }

    public void UpdateAdReward()
    {
        StartCoroutine(UpdateResources("plus", 100));
    }

    IEnumerator UpdateResources(string action, int coins)
    {
        string usr = PlayerPrefs.GetString("username");
        string pswd = PlayerPrefs.GetString("password");

        JSONObject js = new JSONObject();
        js.AddField("user_name", usr);
        js.AddField("password", pswd);
        js.AddField("action", action);
        js.AddField("coins", coins.ToString());

        using (UnityWebRequest webRequest = new UnityWebRequest("http://server.wordsfight.com/update-coins", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(js.ToString());
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                JSONObject response = new JSONObject(webRequest.downloadHandler.text);
                Debug.Log("Received: " + response.ToString());

                if (response.HasField("message"))
                {
                    if (action == "plus")
                    {
                        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + coins);
                        PlayerPrefs.SetInt("weekly_rounds", PlayerPrefs.GetInt("weekly_rounds") + 1);
                        PlayerPrefs.SetInt("xp", PlayerPrefs.GetInt("xp") + 20);
                    }
                    else if (action == "minus")
                    {
                        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - coins);
                    }
                }
                else
                {
                    Debug.LogError("Post Sucessful but returned result was unexpected.");
                }
            }
        }
    }

}





/*
    private void SendData(byte[] bb)
    {
        //Sending Data to Server and Calling it's Track Event Listner
        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data["image"] = Convert.ToBase64String(bb);
        //_socket.Emit("Track", new JSONObject(data));
    }



    void OnOpen(TcpIOComponent obj)
    {
        Debug.Log("*******************************Conneted to SERVER ***************************************8");
        
        connection_state = true;
        
        JSONObject jsonObject = new JSONObject();
        jsonObject.AddField("UID", username);
        jsonObject.AddField("MODE", "_CONEC_");
        jsonObject.AddField("PEER", "");
        Debug.Log(jsonObject.str);
        _socket.Emit(jsonObject.str);
        

    }

    void OnError(TcpIOComponent obj)
    {
        Debug.Log("------------------Connection ERROR -----------------");
        connection_state = false;
    }
    void OnClose(SocketIOEvent obj)
    {
        Debug.Log("------------------Connection CLOSED -----------------");
        connection_state = false;
    }

    void OnResponse(TcpIOComponent obj)
    {
        
       //Recieving Response from Server
       JSONObject jsonObject = obj.data;
       //msgString = (float.Parse(jsonObject.GetField("tracking_angle").str)).ToString();
       //print(msgString);
       //even = true;
       Debug.Log("Data from Server : " + obj.data);
      
    } */
