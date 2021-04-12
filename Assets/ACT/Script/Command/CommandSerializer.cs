using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class CommandSerializer
{
    MemoryStream mMemoryStream;
    BinaryWriter mBinaryWriter;
    BinaryReader mBinaryReader;

    public CommandSerializer()
    {
        mMemoryStream = new MemoryStream();
        mBinaryWriter = new BinaryWriter(mMemoryStream);
    }

    public CommandSerializer(byte[] buffer)
    {
        mMemoryStream = new MemoryStream(buffer);
        mBinaryReader = new BinaryReader(mMemoryStream);
    }

    public bool Saving { get { return mBinaryWriter != null; } }
    public bool Loading { get { return mBinaryReader != null; } }

    public byte[] FetchBytes()
    {
        if (mBinaryWriter != null)
            mBinaryWriter.Close();
        if (mBinaryReader != null)
            mBinaryReader.Close();
        return mMemoryStream.ToArray();
    }

    public virtual void Ser(ref byte value)
    {
        if (Loading)
            value = mBinaryReader.ReadByte();
        else
            mBinaryWriter.Write(value);
    }

    public virtual void Ser(ref float value)
    {
        if (Loading)
            value = mBinaryReader.ReadSingle();
        else
            mBinaryWriter.Write(value);
    }

    public virtual void Ser(ref Int32 value)
    {
        if (Loading)
            value = mBinaryReader.ReadInt32();
        else
            mBinaryWriter.Write(value);
    }

    public virtual void Ser(ref UInt32 value)
    {
        if (Loading)
            value = mBinaryReader.ReadUInt32();
        else
            mBinaryWriter.Write(value);
    }

    public virtual void Ser(ref Int16 value)
    {
        if (Loading)
            value = mBinaryReader.ReadInt16();
        else
            mBinaryWriter.Write(value);
    }
}