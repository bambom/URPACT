using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBinaryCommand
{
    void Serialize(CommandSerializer ser);
}

public class BinaryCommand : IBinaryCommand
{
    public byte OpCode;
    public BinaryCommand(byte op) { OpCode = op; }
    public BinaryCommand() { }

    public virtual int NeedAck { get { return 0; } }
    public virtual bool NeedOrder { get { return false; } }

    public virtual void Serialize(CommandSerializer ser) { ser.Ser(ref OpCode); }
    public void Parse(byte[] data)
    {
        CommandSerializer ser = new CommandSerializer(data);
        Serialize(ser);
    }
}

public class CreateGameRequest : BinaryCommand
{
    public UInt32 GameId;
    public UInt32 GamePass;

    public override int NeedAck { get { return 100; } }

    public CreateGameRequest(int gameId, int gamePass)
        : base(NetCommon.USER_CREATE_GAME)
    {
        GameId = (UInt32)gameId;
        GamePass = (UInt32)gamePass;
    }
    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref GameId);
        ser.Ser(ref GamePass);
    }
}

public class JoinGameRequest : BinaryCommand
{
    public UInt32 GameId;
    public UInt32 GamePass;
    public UInt32 UserID;

    public override int NeedAck { get { return 100; } }

    public JoinGameRequest() { }
    public JoinGameRequest(int gameId, int gamePass, int userId)
        : base(NetCommon.USER_JOIN_GAME)
    {
        GameId = (UInt32)gameId;
        GamePass = (UInt32)gamePass;
        UserID = (UInt32)userId;
    }
    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref GameId);
        ser.Ser(ref GamePass);
        ser.Ser(ref UserID);
    }
}

public class LeaveGameRequest : BinaryCommand
{
    public UInt32 UserID;

    public override int NeedAck { get { return 100; } }

    public LeaveGameRequest() { }
    public LeaveGameRequest(int userId) : base(NetCommon.USER_LEAVE_GAME)
    {
        UserID = (UInt32)userId;
    }
    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref UserID);
    }
}

public class ServerResponse : BinaryCommand
{
    public byte Response;
    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Response);
    }
}

public class MasterChangeResponse : BinaryCommand
{
    public UInt32 Master;
    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Master);
    }
}

public class MessageRequest : BinaryCommand
{
    public byte Id;
    public MessageRequest() { }
    public MessageRequest(MessageId id)
        : base(NetCommon.USER_MESSAGES) 
    {
        Id = (byte)id;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Id);
    }
}

public class PlayerActionRequest : MessageRequest
{
    public UInt32 UserID;
    public Int16 X;
    public Int16 Y;
    public Int16 Z;
    public Int16 Dir;
    public byte Action;

    public virtual bool NeedOrder { get { return true; } }

    public PlayerActionRequest() { }
    public PlayerActionRequest(UInt32 user, Int16 x, Int16 y, Int16 z, Int16 dir, byte action)
        : base(MessageId.PlayAction)
    {
        UserID = user;
        X = x;
        Y = y;
        Z = z;
        Dir = dir;
        Action = action;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref UserID);
        ser.Ser(ref X);
        ser.Ser(ref Y);
        ser.Ser(ref Z);
        ser.Ser(ref Dir);
        ser.Ser(ref Action);
    }
}

public class SyncPlayerRequest : MessageRequest
{
    public UInt32 UserID;
    public Int16 X;
    public Int16 Y;
    public Int16 Z;
    public Int16 Dir;
    public byte Action;
    public byte Master;

    public virtual bool NeedOrder { get { return true; } }

    public SyncPlayerRequest() { }
    public SyncPlayerRequest(UInt32 user, Int16 x, Int16 y, Int16 z, Int16 dir, byte action, byte master)
        : base(MessageId.SyncPlayer)
    {
        UserID = user;
        X = x;
        Y = y;
        Z = z;
        Dir = dir;
        Action = action;
        Master = master;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref UserID);
        ser.Ser(ref X);
        ser.Ser(ref Y);
        ser.Ser(ref Z);
        ser.Ser(ref Dir);
        ser.Ser(ref Action);
        ser.Ser(ref Master);
    }
}

public class PlayerSyncMoveRequest : MessageRequest
{
    public UInt32 UserID;
    public Int16 X;
    public Int16 Z;

    public virtual bool NeedOrder { get { return true; } }

    public PlayerSyncMoveRequest() { }
    public PlayerSyncMoveRequest(UInt32 user, Int16 x, Int16 z)
        : base(MessageId.SyncMove)
    {
        UserID = user;
        X = x;
        Z = z;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref UserID);
        ser.Ser(ref X);
        ser.Ser(ref Z);
    }
}

public class PlayerSyncRotateRequest : MessageRequest
{
    public UInt32 UserID;
    public Int16 Rotate;

    public virtual bool NeedOrder { get { return true; } }

    public PlayerSyncRotateRequest() { }
    public PlayerSyncRotateRequest(UInt32 user, Int16 rotate)
        : base(MessageId.SyncRotate)
    {
        UserID = user;
        Rotate = rotate;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref UserID);
        ser.Ser(ref Rotate);
    }
}

public class HitDataRequest : MessageRequest
{
    public HitData HitData = new HitData();

    public virtual bool NeedOrder { get { return true; } }

    public HitDataRequest() { }
    public HitDataRequest(HitData hitData)
        : base(MessageId.HitData)
    {
        HitData = hitData;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        HitData.Serialize(ser);
    }
}

public class DeadRequest : MessageRequest
{
    public UInt32 Target;

    public override int NeedAck { get { return 100; } }

    public DeadRequest() { }
    public DeadRequest(UInt32 target)
        : base(MessageId.Dead)
    {
        Target = target;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Target);
    }
}

public class UpdateUserAttribRequest : MessageRequest
{
    public UInt32 Target;
    public int HP;
    public int Ability;
    public int Soul;
    public UpdateUserAttribRequest() { }
    public UpdateUserAttribRequest(UInt32 target, int hp, int ability, int soul)
        : base(MessageId.UpdateUserInfo)
    {
        Target = target;
        HP = hp;
        Ability = ability;
        Soul = soul;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Target);
        ser.Ser(ref HP);
        ser.Ser(ref Ability);
        ser.Ser(ref Soul);
    }
}

public class MasterGameStatedRequest : MessageRequest
{
    public UInt32 Master;
    public MasterGameStatedRequest() { }
    public MasterGameStatedRequest(UInt32 master)
        : base(MessageId.MasterGameStated)
    {
        Master = master;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Master);
    }
}

public class MasterGameFinishedRequest : MessageRequest
{
    public UInt32 Master;
    public byte Win;
    public MasterGameFinishedRequest() { }
    public MasterGameFinishedRequest(UInt32 master, bool win)
        : base(MessageId.MasterGameFinished)
    {
        Master = master;
        Win = win ? (byte)1 : (byte)0;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Master);
        ser.Ser(ref Win);
    }
}

public class BuffRequest : MessageRequest
{
    public UInt32 Target;
    public int Id;

    public override int NeedAck { get { return 50; } }

    public BuffRequest() { }
    public BuffRequest(UInt32 target, int id)
        : base(MessageId.Buff)
    {
        Target = target;
        Id = id;
    }

    public override void Serialize(CommandSerializer ser)
    {
        base.Serialize(ser);
        ser.Ser(ref Target);
        ser.Ser(ref Id);
    }
}