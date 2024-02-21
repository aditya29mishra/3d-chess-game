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
    private void Init(string ip, ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);
        endpoint.Port = port;

        connections = driver.Connect(endpoint);
        Debug.Log("Attemping to connect to the server on " + endpoint.Address);
        isActive = true;
        //RegisterToEvent();
    }
    public void ShutDown()
    {
        //UnRegisterToEvent();

        driver.Dispose();
        isActive = false;
        connections = default(NetworkConnection);

    }
    public void OnDestroy()
    {
        ShutDown();
    }

    public void Update()
    {
        if (!isActive)
        {
            return;
        }

        //keepAlive();
        driver.ScheduleUpdate().Complete();

        CheckAlive();


        // Update existing connections
        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!connections.IsCreated && isActive)
        {
            Debug.Log("Something is wrong , lost conection to server");
            connectionDropped?.Invoke();
            ShutDown();
        }
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream;

        NetworkEvent.Type cmd;

        while ((cmd = connections.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty) ;
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                //SentToServer(new Netwelcome());
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                //NetUtility.Ondata(stream, default(NetworkConnection));
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected form server");
                connections = default(NetworkConnection);
                connectionDropped?.Invoke();
                ShutDown()l 
            }
        }

    }
    public void sendToServer(NetworkConnection msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connections, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer)
    }
}
