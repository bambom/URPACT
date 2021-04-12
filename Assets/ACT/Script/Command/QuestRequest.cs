using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class QuestList : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public int[] quests { get; set; }
}

[ProtoBuf.ProtoContract]
public class GetQuestListCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class FetchQuestRewardCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }
    
    public FetchQuestRewardCmd() { }
    public FetchQuestRewardCmd(int _id)
    {
        id = _id;
    }
}