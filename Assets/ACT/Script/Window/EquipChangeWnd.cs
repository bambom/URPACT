using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipChangeWnd : Window<EquipChangeWnd>
{
	public override string PrefabName { get { return "EquipChangeWnd"; } }
	
	Player mCharactor;
	Player mOtherCharactor;
	GameObject mCharactorRoot;
	
	public Player Charactor{ get{return mCharactor ;}}
	public GameObject CharactorRoot{ get{ return Control("CharactorRotate"); }}
	public GameObject OtherCharactorRoot{ get{return Control ("OtherCharactorRotate"); }}
	
	#region Protected
	protected override bool OnOpen()
    {
		Init();
        return base.OnOpen();
    }
	
    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	
	public override void Hide()
	{
		base.Hide();
	}
	
	public override void Show()
	{
		if (mCharactor != null)
			mCharactor.PlayAction(Data.CommonAction.ShowEquip);
		if (mOtherCharactor != null)
			mOtherCharactor.PlayAction(Data.CommonAction.ShowEquip);
		base.Show();
	}
	
	#endregion

	void Init()
	{
		//ResetParent For reason It is A 3D camera
		GameObject go = GameObject.Find("WindowsRoot");
		WndObject.transform.parent = go.transform;	
		WndObject.transform.localPosition = new Vector3(0.0f,10000.0f,0.0f);
		mCharactor = Player.CreateShowPlayer(PlayerDataManager.Instance.Data, CharactorRoot);
		
		if (mCharactor != null)
			mCharactor.PlayAction(Data.CommonAction.ShowEquip);
	}
	
	void UpdateCharactor()
	{
        mCharactor.Equip();
	}

	public void Equip()
	{
		UpdateCharactor();
	}
	
	public void ResetPos(Vector3 vNewPos)
	{
		CharactorRoot.transform.parent.transform.localPosition = vNewPos;
	}
	
	public void ResetPos(Vector3 vNewPos, Vector3 vNewScale)
	{
		CharactorRoot.transform.parent.transform.localPosition = vNewPos;
		CharactorRoot.transform.parent.transform.localScale = vNewScale;
	}

	public void OtherResetPos(Vector3 vNewPos)
	{
		OtherCharactorRoot.transform.parent.transform.localPosition = vNewPos;
	}
	
	public void OtherResetPos(Vector3 vNewPos, Vector3 vNewScale)
	{
		OtherCharactorRoot.transform.parent.transform.localPosition = vNewPos;
		OtherCharactorRoot.transform.parent.transform.localScale = vNewScale;
	}
	
	public void CreateOtherPlayer(PlayerData playerData)
	{
		mOtherCharactor = Player.CreateShowPlayer(playerData,OtherCharactorRoot);
		if (mOtherCharactor != null)
			mOtherCharactor.PlayAction(Data.CommonAction.ShowEquip);
	}
	
	public void UpdateCharactor(PlayerData playerData)
	{
		if(mCharactor != null){
			GameObject.Destroy(mCharactor.UGameObject);
			mCharactor = null;
		}
		mCharactor = Player.CreateShowPlayer(playerData,CharactorRoot);
		if (mCharactor != null)
			mCharactor.PlayAction(Data.CommonAction.ShowEquip);
	}
}
