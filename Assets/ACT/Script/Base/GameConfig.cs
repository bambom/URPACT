using UnityEngine;
using System.Collections.Generic;

public class GameConfig 
{
    public class UpdateFile
    {
        public string file;
        public int ver;
    }

    public class ServerInfo
    {
        public string server;
        public string name;
        public int port;
        public bool recommend;
    }

    public UpdateFile[] files;
    public Dictionary<string, ServerInfo> serverlist;
    public string link;
    public string linkName;
    public string linkCaption;
    public string linkDescription;
    public string picture;
}
