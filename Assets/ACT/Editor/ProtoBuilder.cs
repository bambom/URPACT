using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System;

public class ProtoBuilder : ScriptableWizard
{
    public bool ResetCommands = false;
    public string RequestTypeFile = Application.dataPath + "/Script/Command/RequestTypes.cs";

    [MenuItem("GameObject/Engine/ProtoBuilder")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ProtoBuilder>("ProtoBuilder", "Build", "Test");
    }

    bool BuildRequestTypes(List<string> requestTypes)
    {
        requestTypes.Sort();

        int enumMax = (int)ERequestTypes.EMax;
        List<string> newAddedTypes = new List<string>();
        SortedDictionary<int, string> enumIndexMap = new SortedDictionary<int, string>();
        if (ResetCommands)
        {
            enumMax = 0;
            newAddedTypes.AddRange(requestTypes);
        }
        else
        {
            List<string> enumNames = new List<string>(Enum.GetNames(typeof(ERequestTypes)));
            foreach (string name in requestTypes)
            {
                if (enumNames.Contains(name))
                {
                    int enumIndex = (int)(ERequestTypes)Enum.Parse(typeof(ERequestTypes), name);
                    enumIndexMap[enumIndex] = name;
                }
                else
                    newAddedTypes.Add(name);
            }
        }

        foreach (string name in newAddedTypes)
            enumIndexMap[enumMax++] = name;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("//Autogen by ProtoBuilder, do not edit it manualy.");
        sb.AppendLine("");
        sb.AppendLine("public enum ERequestTypes");
        sb.AppendLine("{");
        foreach (KeyValuePair<int, string> pair in enumIndexMap)
            sb.AppendFormat("    {0} = {1},", pair.Value, pair.Key).AppendLine();
        sb.AppendFormat("    {0} = {1},", ERequestTypes.EMax, enumMax).AppendLine();
        sb.AppendLine("}");

        string text = sb.ToString();
        if (File.ReadAllText(RequestTypeFile) == text)
            return false;

        File.WriteAllText(RequestTypeFile, text);

        AssetDatabase.Refresh();

        return true;
    }

    void OnWizardCreate()
    {
        StringBuilder sb = new StringBuilder();
        Type baseType = typeof(ICommand);
        Type requestType = typeof(RequestCmd);
        List<string> requestTypes = new List<string>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (baseType.IsAssignableFrom(type))
                    sb.AppendLine(ProtoBuf.Serializer.GetProto(type));

                if (requestType.IsAssignableFrom(type))
                    requestTypes.Add("E" + type.Name);
            }
        }

        // if the file is changed, do exist now.
        if (BuildRequestTypes(requestTypes))
            return;

        // process duplicate message bodys.
        string totalProto = sb.ToString();
        string[] split = { "message " };
        string[] messageBodys = totalProto.Split(split, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> messageBodyMap = new Dictionary<string, string>();
        foreach (string messageBody in messageBodys)
        {
            if (messageBody.Trim().Length == 0)
                continue;

            string name = messageBody.Substring(0, messageBody.IndexOf(' '));
            messageBodyMap[name] = messageBody;
        }

        // rebuild the protos.
        totalProto = "";
        foreach (string body in messageBodyMap.Values)
            totalProto += "message " + body;

        string fileName = EditorUtility.SaveFilePanel(
                    "Save the proto file",
                    "",
                    "command",
                    "proto");
        if (fileName.Length > 0)
            File.WriteAllText(fileName, totalProto);
    }

    void OnWizardOtherButton()
    {
        CommandBuilder buidler = new CommandBuilder();
        byte[] cmd = buidler.Build(new ServerTestRequest());

        string output = "";
        foreach (byte b in cmd)
            output += b + ",";
        Debug.Log(output);
    }
}
