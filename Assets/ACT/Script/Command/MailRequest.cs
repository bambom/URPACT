using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class Mail
{
    [ProtoBuf.ProtoMember(1)]
    public string title { get; set; } // 邮件的头

    [ProtoBuf.ProtoMember(2)]
    public string sender { get; set; } // 发送者

    [ProtoBuf.ProtoMember(3)]
    public string content { get; set; } // 正文【邮件列表无】

    [ProtoBuf.ProtoMember(4)]
    public int item { get; set; } // 附件物品【邮件列表无】

    [ProtoBuf.ProtoMember(5)]
    public int gold { get; set; } // 附件银币【邮件列表无】

    [ProtoBuf.ProtoMember(6)]
    public int gem { get; set; } // 附件金币【邮件列表无】

    [ProtoBuf.ProtoMember(7)]
    public long time { get; set; } // 发送时间

    [ProtoBuf.ProtoMember(8)]
    public bool attach { get; set; } // 是否包含附件【邮件列表有】

    [ProtoBuf.ProtoMember(9)]
    public int flag { get; set; } // 是否已读,0-未读，1-已读
}

[ProtoBuf.ProtoContract]
public class GetMailResponse : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public int total { get; set; } // 总的邮件条数。

    [ProtoBuf.ProtoMember(2)]
    public int page { get; set; } // 当前的页码

    [ProtoBuf.ProtoMember(3)]
    public Mail[] mails { get; set; } // 当前页邮件的列表。
}

[ProtoBuf.ProtoContract]
public class GetMailsCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int page { get; set; } // 总的邮件条数。

    [ProtoBuf.ProtoMember(2)]
    public int size { get; set; } // 当前的页码

    public GetMailsCmd() { }
    public GetMailsCmd(int inPage, int inSize) { page = inPage; size = inSize; }
}

[ProtoBuf.ProtoContract]
public class ReadMailCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int mail { get; set; } 

    public ReadMailCmd() { }
    public ReadMailCmd(int inMail) { mail = inMail; }
}

[ProtoBuf.ProtoContract]
public class FetchAttachCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int mail { get; set; } 

    public FetchAttachCmd() { }
    public FetchAttachCmd(int inMail) { mail = inMail; }
}

[ProtoBuf.ProtoContract]
public class DeleteMailCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int mail { get; set; } 

    public DeleteMailCmd() { }
    public DeleteMailCmd(int inMail) { mail = inMail; }
}

[ProtoBuf.ProtoContract]
public class SendMailCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string to { get; set; } 

    [ProtoBuf.ProtoMember(2)]
    public string title { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public string content { get; set; }

    public SendMailCmd(string _to, string _title, string _content)
    {
        to = _to;
        title = _title;
        content = _content;
    }
}