using Assets.Scripts.Net;
using Unity.Networking.Transport;
public class NetKeepAlive : NetMessage
{
    public NetKeepAlive()
    {
        code = OpCode.KEEP_ALIVE;
    }
    public NetKeepAlive(DataStreamReader reader)
    {
        code = OpCode.KEEP_ALIVE;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)code);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }

}