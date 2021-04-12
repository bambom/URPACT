using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class FriendList : ICommand
{
    [ProtoBuf.ProtoContract]
    public class FriendInfo : ICommand
    {
        [ProtoBuf.ProtoMember(1)]
        public string name { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int role { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public bool online { get; set; }
    }

    [ProtoBuf.ProtoMember(1)]
    public FriendInfo[] friends { get; set; }
}

[ProtoBuf.ProtoContract]
public class GetFriendsCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class AddFriendCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string friend { get; set; }
    
    public AddFriendCmd() { }
    public AddFriendCmd(string _user)
    {
        friend = _user;
    }
}

[ProtoBuf.ProtoContract]
public class DeleteFriendCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string friend { get; set; }
    
    public DeleteFriendCmd() { }
    public DeleteFriendCmd(string _user)
    {
        friend = _user;
    }
}