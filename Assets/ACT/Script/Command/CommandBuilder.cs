using System;
using System.IO;
using System.Text;

public class CommandBuilder
{
    public byte[] Build(RequestCmd command)
    {
        MemoryStream stream = new MemoryStream();
        short msgId = (short)(ERequestTypes)Enum.Parse(typeof(ERequestTypes), "E" + command.GetType().Name);
        stream.WriteByte(0);
        stream.WriteByte(0);
        stream.WriteByte((byte)(msgId & 0xff));
        stream.WriteByte((byte)(msgId >> 8));

        ProtoBuf.Serializer.Serialize<RequestCmd>(stream, command);  //序列化
        return stream.ToArray();
    }
}
