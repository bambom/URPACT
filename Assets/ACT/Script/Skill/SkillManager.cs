using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillDataManager : Singleton<SkillDataManager> {
	
	List<SkillItem>mSkillItemList = new List<SkillItem>();
	public bool HasSkill(int skillID)
	{
		foreach (Item item in PlayerDataManager.Instance.SkillPack.Items.Values)
        {
            SkillItem skillItem = item.ToSkillItem();
            if (skillItem == null)
                continue;
			if(skillItem.Item.ID == skillID)
				return true;
        }
		return false;
	}
	
	public SkillItem GetSkillItem(int skillID)
	{
		foreach (SkillItem item in mSkillItemList )
        {
			if(item.Item.ID == skillID)
				return item;
        }
		return null;
	}
	
	public void UpdateSkillItem()
	{
		mSkillItemList.Clear();
		foreach (Item item in PlayerDataManager.Instance.SkillPack.Items.Values)
        {
            SkillItem skillItem = item.ToSkillItem();
            if (skillItem == null)
                continue;
			mSkillItemList.Add(skillItem);
        }
	}
	
	public SkillItem[] GetEquipSkillItem()
	{
		List<SkillItem>skillArray = new List<SkillItem>();
		foreach (Item item in PlayerDataManager.Instance.SkillPack.Items.Values)
        {
            SkillItem skillItem = item.ToSkillItem();
            if (skillItem == null || skillItem.SkillBase == null)
                continue;
			if(skillItem.Equiped)
				skillArray.Add(skillItem);	
        }
		return skillArray.ToArray();
	}
	
	public bool HasActivationSkill(int skillId)
	{
		SkillItem skill = GetSkillItem(skillId);
		SkillItem[] skillArray = GetEquipSkillItem();
	    foreach (SkillItem skillItem in skillArray)
       	{
			if ( skill.Item.ID == skillItem.Item.ID )
				return true;
        }
		return false;
	}
	
	// can learn but not learning.
	public List<ItemBase> GetCanLearnSkill()
	{
		List<ItemBase> canLearnSkill = new List<ItemBase>();
		SkillAttrib attrib = null;
		ItemBase[] itemArray = ItemBaseManager.Instance.GetAllItem();
		foreach (ItemBase item in itemArray) {
			int role = PlayerDataManager.Instance.Attrib.Role;
			if(role == item.Role && item.MainType == (int)EItemType.Skill && item.SubType != 2)
			{
				attrib = SkillAttribManager.Instance.GetItem(item.ID, 1);
				if( attrib != null )
				{
					if(!HasSkill(item.ID))
					{	
						if(attrib.LevelRequest <= PlayerDataManager.Instance.Attrib.Level)
							canLearnSkill.Add(item);
					}
					else
					{
						SkillItem skillItem = GetSkillItem(item.ID);
						SkillAttrib sa = SkillAttribManager.Instance.GetItem(item.ID, skillItem.Lv+1);
						if(sa != null && sa.LevelRequest <= PlayerDataManager.Instance.Attrib.Level)
							canLearnSkill.Add(item);
					}

				}
			}
		}
		return canLearnSkill;
	}
}
