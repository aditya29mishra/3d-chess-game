using UnityEngine;
using Unity.Networking.Transport;


public class NetWelcome : NetMessage
{
    public int AssingedTeam { set; get; }
    public NetWelcome()
    {
        Code = OpCode.WELCOME;
    }

    public NetWelcome(DataStreamReader reader)
    {
        Code = OpCode.WELCOME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(AssingedTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        AssingedTeam = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        Debug.Log("ReceivedOnClient is about to trigger C_WELCOME event...");
        NetUtility.C_WELCOME?.Invoke(this);
        Debug.Log("C_WELCOME event triggered in ReceivedOnClient.");
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        Debug.Log("ReceivedOnServer is about to trigger S_WELCOME event...");
        NetUtility.S_WELCOME?.Invoke(this, cnn);
        Debug.Log("S_WELCOME event triggered in ReceivedOnServer.");
    }
}

