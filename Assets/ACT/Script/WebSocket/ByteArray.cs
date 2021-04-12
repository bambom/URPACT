using System.Collections;
using System;
/**
 *@author <a href="mailto:bowen@corp.shadowgame.cn">liubowen<a>
 *2013/10/31 20:26:45
 */
public sealed class ByteArray
{

    private int readPos;
    private int writePos;

    private byte[] data;
    public void position(int i)
    {
        readPos = writePos = i;
    }

    public void skipBytes(int i)
    {
        readPos += i;
    }

    public int capacity()
    {
        return data.Length;
    }

    private void ensureCapacity(int i)
    {
        if (i > data.Length)
        {
            byte[] abyte0 = new byte[(i * 3) / 2];
            System.Array.Copy(data, 0, abyte0, 0, writePos);
            data = abyte0;
        }
    }


    public void pack()
    {
        if (readPos == 0)
            return;
        int i = available();
        for (int j = 0; j < i; j++)
            data[j] = data[readPos++];

        readPos = 0;
        writePos = i;
    }

    public void writeByte(int i)
    {
        writeNumber(i, 1);
    }

    public int readByte()
    {
        return data[readPos++];
    }

    public int readUnsignedByte()
    {
        return data[readPos++] & 0xff;
    }

    private void read(byte[] abyte0, int i, int j, int k)
    {


        System.Array.Copy(data, k, abyte0, i, j);


    }

    public int getReadPos()
    {
        return readPos;
    }

    public void setReadPos(int i)
    {
        readPos = i;
    }

    private void write(byte[] abyte0, int i, int j, int k)
    {
        ensureCapacity(k + j);

        System.Array.Copy(abyte0, i, data, k, j);
    }

    public void writeChar(char c)
    {
        writeNumber(c, 2);
    }

    private void writeNumber(long l, int i)
    {
        ensureCapacity(writePos + i);

        for (int j = 0; j < i; j++)
        {
            data[writePos++] = (byte)((l >> (8 * ((i - 1) - j))) & 0xFF);
        }
    }


    public byte[] getBytes()
    {
        byte[] abyte0 = new byte[length()];

        System.Array.Copy(data, 0, abyte0, 0, abyte0.Length);
        return abyte0;
    }

    public ByteArray clone()
    {
        ByteArray bytebuffer = new ByteArray(writePos);

        System.Array.Copy(data, 0, bytebuffer.data, 0, writePos);
        bytebuffer.writePos = writePos;
        bytebuffer.readPos = readPos;
        return bytebuffer;
    }

    public int length()
    {
        return writePos;
    }

    public void writeBoolean(bool flag)
    {
        writeByte(flag ? 1 : 0);
    }

    public bool readBoolean()
    {
        return readByte() != 0;
    }



    public void reset()
    {
        readPos = 0;
    }

    public void writeLong(long l)
    {
        writeNumber(l, 8);
    }

    public ByteArray()
        : this(1024)
    {

    }

    public ByteArray(int i)
    {
        data = new byte[i];
    }

    public ByteArray(byte[] abyte0)
        : this(abyte0, 0, abyte0.Length)
    {


    }

    public ByteArray(byte[] abyte0, int i, int j)
    {
        data = abyte0;
        readPos = i;
        writePos = i + j;
    }


    public long readLong()
    {
        long high = (uint)readInt();
        long low = (uint)readInt();
        return (high << 32) | low;

    }

    public void writeShort(int i)
    {
        writeNumber(i, 2);
    }

    public int available()
    {
        return writePos - readPos;
    }

    public short readShort()
    {
        byte high = (byte)readByte();
        byte low = (byte)readByte();
        return (short)(high << 8 | low);
    }

    public void writeByteBuffer(ByteArray byteBuff)
    {
        writeByteBuffer(byteBuff, byteBuff.available());
    }

    public void writeByteBuffer(ByteArray byteBuff, int i)
    {
        ensureCapacity(length() + i);
        for (int j = 0; j < i; j++)
            data[writePos++] = byteBuff.data[byteBuff.readPos++];
    }

    public void writeBytes(byte[] abyte0)
    {
        writeBytes(abyte0, 0, abyte0.Length);
    }

    public void writeBytes(byte[] abyte0, int i, int j)
    {
        ensureCapacity(writePos + j);
        for (int k = 0; k < j; k++)
            data[writePos++] = abyte0[i++];

    }

    public byte[] readBytes(int i)
    {
        byte[] abyte0 = new byte[i];
        for (int j = 0; j < i; j++)
            abyte0[j] = data[readPos++];

        return abyte0;
    }

    public int readUnsignedShort()
    {
        return (int)(readShort() & 65535L);
    }

    public string toString()
    {
        return System.Text.Encoding.UTF8.GetString(data, 0, writePos);
    }

    public int getWritePos()
    {
        return writePos;
    }

    public void setWritePos(int i)
    {
        writePos = i;
    }

    public byte[] getRawBytes()
    {
        return data;
    }

    public void writeUTF(string s)
    {
		ushort len = (ushort)System.Text.Encoding.UTF8.GetByteCount(s);
		if (s == null || len == 0)
        {
            writeShort(0);
            return;
        }
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
        if (len > 65535)
            throw new Exception("the string is too long:" + len);
        ensureCapacity(writePos + len + 2);
        writeShort(len);
        writeBytes(bytes);
    }


      


//    public string readUTF()
//    {
//        int i = readUnsignedShort();
//        if(i == 0)
//            return "";
//        char[] ac = new char[i];
//        int j = 0;
//        for(int l = readPos + i; readPos < l;)
//        {
//            int k = data[readPos++] & 0xff;
//            if(k < 127)
//                ac[j++] = (char)k;
//            else
//            if(k >> 5 == 7)
//            {
//                byte byte0 = data[readPos++];
//                byte byte2 = data[readPos++];
//                ac[j++] = (char)((k & 0xf) << 12 | (byte0 & 0x3f) << 6 | byte2 & 0x3f);
//            } else
//            {
//                byte byte1 = data[readPos++];
//                ac[j++] = (char)((k & 0x1f) << 6 | byte1 & 0x3f);
//            }
//        }
//
//        return new string(ac, 0, j);
//    }
    public String readUTF()
    {
        int len = readUnsignedShort();
        if (len == 0)
            return "";
        byte[] bytes = this.readBytes(len);
        return System.Text.Encoding.UTF8.GetString(bytes, 0, len);
    }

    public void clear()
    {
        writePos = readPos = 0;
    }

    public void writeInt(int i)
    {
        writeNumber(i, 4);
    }

    public int readInt()
    {
        int high = (ushort)readShort();
        int low = (ushort)readShort();
        return (high << 16) | low;
    }

    public int position()
    {
        return readPos;
    }

}
