using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    #region Single implementation 
    public static Server Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastKeepAlive;

    private Action connectionDropped;
    // Methods
    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = port;

        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port " + endpoint.Port);
            return;
        }
        else
        {
            driver.Listen();
            Debug.Log("Server listening on port " + endpoint.Port);
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;

    }
    public void ShutDown()
    {
        driver.Dispose();
        connections.Dispose();
        isActive = false;

    }
    public void OnDestroy()
    {
        ShutDown();
    }
    public void Update()
    {
        if (!isActive){
            return;
        }
        
        keepAlive();

        driver.ScheduleUpdate().Complete();

        // Clean up connections
        CleanUpConnections();

        // Accept new connections
        AcceptNewConnections();

        // Update existing connections
        UpdateConnections();
    }

    private void keepAlive()
    {
        if (Time.time - lastKeepAlive > keepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }

    private void CleanUpConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }
    private void AcceptNewConnections()
    {
        NetworkConnection c;

        while((c = driver.Accept())  != default(NetworkConnection))
        {
            connections.Add(c);
        }
    }
    private void UpdateConnections()
    {
        DataStreamReader stream;
        for(int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;

            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                  NetUtility.Ondata(stream, connections[i], this);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log( "client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    ShutDown(); // as its 2 person game, we can just shut down the server
                }
            }
        }
    }
    
    //server specific 
    public void SendToClient(NetworkConnection Connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend (Connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
    public void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
            //    Debug.Log($"sending {msg.Code} to : { connections[i].InternalId}");
                SendToClient(connections[i], msg);
            }
        }
    }



}
