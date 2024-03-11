using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    #region Single implementation 
    public static Client Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NetworkConnection connections;

    private bool isActive = false;

    private Action connectionDropped;


    // Methods
    public void Init(string ip, ushort port)
    {
        Debug.Log("Initializing client...");
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);
        endpoint.Port = port;

        connections = driver.Connect(endpoint);
        Debug.Log("Attemping to connect to the server on " + endpoint.Address);
        isActive = true;
        RegisterToEvent();
        Debug.Log("Client initialized.");
    }
    public void ShutDown()
    {
        Debug.Log("Shutting down client...");
        UnRegisterToEvent();

        driver.Dispose();
        isActive = false;
        connections = default(NetworkConnection);
        Debug.Log("Client shut down.");
    }
    public void OnDestroy()
    {
        ShutDown();
    }

    public void Update()
    {
        Debug.Log("Updating client...");
        if (!isActive)
        {
            return;
        }

        //keepAlive();
        driver.ScheduleUpdate().Complete();

        CheckAlive();


        // Update existing connections
        UpdateMessagePump();

        Debug.Log("Client updated.");
    }

    private void CheckAlive()
    {
        Debug.Log("Checking connection status...");
        if (!connections.IsCreated && isActive)
        {
            Debug.Log("Something is wrong , lost conection to server");
            connectionDropped?.Invoke();
            ShutDown();
        }
    }
    private void UpdateMessagePump()
    {
        Debug.Log("Updating message pump...");
        DataStreamReader stream;

        NetworkEvent.Type cmd;

        while ((cmd = connections.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {

                        if (cmd == NetworkEvent.Type.Connect)
                        {
                            sendToServer(new NetWelcome());
                            Debug.Log("Connected to the server");
                        }
                        else if (cmd == NetworkEvent.Type.Data)
                        {
                            NetUtility.Ondata(stream, default(NetworkConnection));
                        }
                        else if (cmd == NetworkEvent.Type.Disconnect)
                        {
                            Debug.Log("Client got disconnected form server");
                            connections = default(NetworkConnection);
                            connectionDropped?.Invoke();
                            ShutDown(); 
                        }
        }

    }
    public void sendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connections, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void RegisterToEvent()
    {
       NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }

    public void UnRegisterToEvent()
    {
       NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm)
    {
        sendToServer(nm);
        Debug.Log("Client received keep alive");
    }
}
