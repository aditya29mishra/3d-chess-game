using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    MAKE_MOVE = 4,
    REMATCH = 5
}

public class NetMessage
{
    public OpCode code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);

    }

    public virtual void Deserialize(ref DataStreamReader reader)
    {
        
    }

    public virtual void ReceivedOnClient()
    {

    }

    public virtual void ReceivedOnServer(NetworkConnection cnn)
    {
        
    }

    
}
