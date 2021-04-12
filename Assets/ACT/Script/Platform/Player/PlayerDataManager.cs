using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    public PlayerData mData = null;//new PlayerData();

    public PlayerData Data { get { return mData; } }
    public MainAttrib Attrib { get { return mData.Attrib; } }
    public Package BackPack { get { return mData.Packages.BackPack; } }
    public Package SkillPack { get { return mData.Packages.SkillPack; } }
	
	public FriendList mFriendsList = new FriendList();
	public FriendList FriendsList { get { return mFriendsList;} }
		 
	public QuestList  mQuestList = new QuestList();
	public QuestList  QuestsList { get { return mQuestList; } }

    public StrengthInfo mStrengthInfo = new StrengthInfo();
    public StrengthInfo StrengthInfo { get { return mStrengthInfo; } }
	
    public void OnPlayerData(PlayerData data)
    {
        mData = data;
        mData.Init();

        Player player = UnitManager.Instance.LocalPlayer as Player;
        if (player != null)
            player.PlayerData = data;
    }

    public void OnMainAttrib(MainAttrib data)
    {
        mData.Attrib = data;
        
        Player player = UnitManager.Instance.LocalPlayer as Player;
        if (player != null)
            player.UpdateAttributes();

        if (InGameMainWnd.Exist)
            InGameMainWnd.Instance.ResourceBar.Update(data);
    }
	
	public void OnBackPack(Package data, bool updateEquip)
	{
		mData.Packages.BackPack = data;
		mData.Packages.BackPack.Init();
		mData.Packages.BackPack.Sort();
        // if the back pack is changed, we need to update the user equip.
        if (updateEquip)
        {
            Player player = UnitManager.Instance.LocalPlayer as Player;
            if (player != null)
                player.Equip();
        }
	}
	
	public void OnSkillPack(Package data)
	{
		mData.Packages.SkillPack = data;
		mData.Packages.SkillPack.Init();
	}
	
	public void OnFriendsList(FriendList data)
	{
		mFriendsList = data;
	}
	
	public void OnQuestsList(QuestList data)
	{
		mQuestList = data;
	}

    public void OnStrengthInfo(StrengthInfo strengthInfo)
	{
        mStrengthInfo = strengthInfo;
	}
}
