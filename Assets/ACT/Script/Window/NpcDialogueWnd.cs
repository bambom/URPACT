using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcDialogueWnd : Window<NpcDialogueWnd>
{
	UnitBase unitBase;
	
    public override string PrefabName { get { return "NpcDialogueWnd"; } }
	
    protected override bool OnOpen()
    {
		UIEventListener.Get(Control("NpcClose")).onClick = OnClickNpcClose;
		
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }

    void OnClickNpcClose(GameObject go)
    {
		NpcDialogueWnd.Instance.Close();
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.Show();
		else
			InGameMainWnd.Instance.Open();
    }
	
	public void InitContent(int NpcId)
    {
		unitBase = UnitBaseManager.Instance.GetItem(NpcId);
		Control("NpcName",Control("MessagePanel")).GetComponent<UILabel>().text = "[ff0000]"+unitBase.Name;
		
		switch(Random.Range(1,4))
		{
		case 1:
			Control("NpcMessage",Control("MessagePanel")).GetComponent<UILabel>().text = "[ff0000]"+unitBase.CustomWord1;
			break;
		case 2:
			Control("NpcMessage",Control("MessagePanel")).GetComponent<UILabel>().text = "[ff0000]"+unitBase.CustomWord2;
			break;
		case 3:
			Control("NpcMessage",Control("MessagePanel")).GetComponent<UILabel>().text = "[ff0000]"+unitBase.CustomWord3;
			break;
		}
		
		if(!string.IsNullOrEmpty(unitBase.Function1))
		{
			UIEventListener.Get(Control("NpcFun1",Control("MessagePanel"))).onClick = onClickFunBtn;
			Control("Label",Control("NpcFun1",Control("MessagePanel"))).GetComponent<UILabel>().text = unitBase.BtnName1;
		}else
		{
			Control("NpcFun1",Control("MessagePanel")).SetActive(false);
		}
		
		if(!string.IsNullOrEmpty(unitBase.Function2))
		{
			UIEventListener.Get(Control("NpcFun2",Control("MessagePanel"))).onClick = onClickFunBtn;
			Control("Label",Control("NpcFun2",Control("MessagePanel"))).GetComponent<UILabel>().text = unitBase.BtnName2;
		}else
		{
			Control("NpcFun2",Control("MessagePanel")).SetActive(false);
		}
		
		if(!string.IsNullOrEmpty(unitBase.Function3))
		{
			UIEventListener.Get(Control("NpcFun3",Control("MessagePanel"))).onClick = onClickFunBtn;
			Control("Label",Control("NpcFun3",Control("MessagePanel"))).GetComponent<UILabel>().text = unitBase.BtnName3;
		}else
		{
			Control("NpcFun3",Control("MessagePanel")).SetActive(false);
		}

    }
	
	void onClickFunBtn(GameObject go)
	{
		string tempFunName="";
		if (go.transform.name == "NpcFun1")
			tempFunName = unitBase.Function1;
		else if(go.transform.name == "NpcFun2")
			tempFunName = unitBase.Function2;
		else if(go.transform.name == "NpcFun3")
			tempFunName = unitBase.Function3;
		
		switch(tempFunName)
		{
			case "CharactorWnd":
				if(NpcDialogueWnd.Exist)
					NpcDialogueWnd.Instance.Close();
				if(!CharactorWnd.Exist)
					CharactorWnd.Instance.Open();
				break;
			
			case "NpcShopWnd":
				if(NpcDialogueWnd.Exist)
					NpcDialogueWnd.Instance.Close();
				if(!NpcShopWnd.Exist)
					NpcShopWnd.Instance.Open();
				break;
			default:
				//NGUIDebug.Log("Npc Function is NULL:"+tempFunName);
				break;
		}
	}
}
