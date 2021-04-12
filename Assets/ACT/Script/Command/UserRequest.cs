using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class RoleList : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public MainAttrib[] users { get; set; }
}

[ProtoBuf.ProtoContract]
public class StrengthInfo : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public int value { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int time { get; set; }
}

[ProtoBuf.ProtoContract]
public class ReLoginCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string id { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public string user { get; set; }

    public ReLoginCmd() { }
    public ReLoginCmd(string _id, string _user) { id = _id; user = _user; }
}

[ProtoBuf.ProtoContract]
public class LoginCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string id { get; set; }

    public LoginCmd() { }
    public LoginCmd(string inId) { id = inId; }
}

[ProtoBuf.ProtoContract]
public class CreateUserCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string user { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int role { get; set; }

    public CreateUserCmd() { }
    public CreateUserCmd(string _user, int _role)
    {
        user = _user;
        role = _role;
    }
}

[ProtoBuf.ProtoContract]
public class DeleteUserCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string user { get; set; }
    
    public DeleteUserCmd() { }
    public DeleteUserCmd(string _user)
    {
        user = _user;
    }
}

[ProtoBuf.ProtoContract]
public class EnterGameCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string user { get; set; }
    
    public EnterGameCmd() { }
    public EnterGameCmd(string _user)
    {
        user = _user;
    }
}

[ProtoBuf.ProtoContract]
public class LeaveGameCmd : RequestCmd
{
	[ProtoBuf.ProtoMember(1)]
    public string user { get; set; }
    
    public LeaveGameCmd() { }
    public LeaveGameCmd(string _user)
    {
        user = _user;
    }
}

[ProtoBuf.ProtoContract]
public class GetUserAttribCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class GetUserDataCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string user { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public bool all_attrib { get; set; }
    
    public GetUserDataCmd() { }
    public GetUserDataCmd(string _user, bool _all_attrib) { user = _user; all_attrib = _all_attrib; }
}

[ProtoBuf.ProtoContract]
public class GetStrengthCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class BuyStrengthCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class GetLoginInfoCmd : RequestCmd
{
    [ProtoBuf.ProtoContract]
    public class LoginInfo : ICommand
    {
        [ProtoBuf.ProtoMember(1)]
        public int SignInCount { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string LastLoginIp { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public bool CanSignIn { get; set; }
    };
}

[ProtoBuf.ProtoContract]
public class SignInCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class FinishNewGuideCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }
	
	public FinishNewGuideCmd(){}
	public FinishNewGuideCmd(int _id)
    {
        id = _id;
    }
}