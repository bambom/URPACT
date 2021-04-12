using UnityEngine;
using System.Collections;


public class PackItemPosInfo
{
	public static int RowMax = 4;
	public static int ColumnMax = 4;
	
	public int Row;
	public int Column;
	
	//( GridCellWidth - RowMax* RowMagin ) = BaseMargin
	//( 500 - 55*2*4) = 60
	public static float BaseMargin = 20.0f;
	public static float RowMargin = 50.0f;
	public static float ColumnMargin = 50.0f;
	
	public float PosZ = 0.0f;
	
	public PackItemPosInfo()
	{
		
	}
}


public class PackPointPosInfo
{
	public static float BaseMargin = 1.0f;
	public static float RowMargin = 28.0f;
	
	public PackPointPosInfo()
	{
		
	}
}


public enum ItemTool{
	Invalid = -1,
	Weapon = 1,
	Helmet,
	Shoulder,
	Armor,
	Bracers,
	Boots,
	Accessory,
	Item,
	Rune,
	Diamond,
	Medicine,
}