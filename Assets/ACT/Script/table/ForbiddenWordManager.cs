using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ForbiddenWord : ITableItem
{
    public SmartInt ID;
    public string Word;
	
    public int Key() { return ID; }
};

public class ForbiddenWordManager : TableManager<ForbiddenWord, ForbiddenWordManager>
{
    public override string TableName() { return "ForbiddenWord"; }
    //public override int MakeKey(ForbiddenWord obj) { return obj.Key(); }

    Regex mRegex = null;
    public ForbiddenWordManager()
    {
        string patten = "SB";
        foreach (ForbiddenWord word in GetAllItem())
            patten += "|" + word.Word.ToUpper();

        mRegex = new Regex(patten);
    }

    public bool IsValid(string input)
    {
        return !mRegex.IsMatch(input.ToUpper().Replace(" ", ""));
    }

    public string EscapeString(string input)
    {
		return mRegex.Replace(input,"*",input.Length);
        //return mRegex.Replace(input,"*");
    }
}
