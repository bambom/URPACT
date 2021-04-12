using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapSprite
{
	public readonly static string[] SpriteArray = 
	{
	"Button17_MiniMap_UnexRoom_01",
	"Button17_MiniMap_UnexRoom_02",
	"Button17_MiniMap_UnexRoom_03",
	"Button17_MiniMap_UnexRoom_04",
	"Button17_MiniMap_UnexRoom_05",
	"Button17_MiniMap_UnexRoom_06",
	"Button17_MiniMap_UnexRoom_07",
	"Button17_MiniMap_UnexRoom_08",
	"Button17_MiniMap_UnexRoom_09",
	"Button17_MiniMap_ExRoom_01",
	"Button17_MiniMap_ExRoom_02",
	"Button17_MiniMap_ExRoom_03",
	"Button17_MiniMap_ExRoom_04",
	"Button17_MiniMap_ExRoom_05",
	"Button17_MiniMap_ExRoom_06",
	"Button17_MiniMap_ExRoom_07",
	"Button17_MiniMap_ExRoom_08",
	"Button17_MiniMap_ExRoom_09",
	};
}
public enum ELevelMapCellState
{
    None,
	In,
    Pass,
	Around,
    Boss
};
public class LevelMapCell
{
	public GameObject MapCellObject;
	public ELevelMapCellState MapCellState;
	GameCellInfo.EDoorFlag mMapDoorFlag;
	UISprite mSprite; 
	
	public void Init(Transform parent, Vector3 position, GameCellInfo.EDoorFlag flag, Quaternion rotate)
	{
		Init(parent, position);
		Quaternion rotation = Quaternion.identity;
		rotation.eulerAngles = new Vector3(0, 0, rotate.eulerAngles.y);
		MapCellObject.transform.localRotation = rotation;
		mMapDoorFlag = flag;
	}
	public void Init(Transform parent, Vector3 position)
	{
		mSprite = MapCellObject.GetComponent<UISprite>();
		MapCellObject.transform.parent = parent;
		MapCellObject.transform.localPosition = position;
		MapCellObject.transform.localScale = new Vector3(48,40,1);
		//mSprite.MakePixelPerfect();
		MapCellObject.SetActive(true);
	}
	
	public void UpdateCellState(ELevelMapCellState state)
	{
		mSprite.spriteName = GetCellSprite(mMapDoorFlag, state);
		MapCellState = state;
	}
	
	string GetCellSprite(GameCellInfo.EDoorFlag doorFlag, ELevelMapCellState state)
	{
		string spriteName = "";
		switch(doorFlag)
		{
			case GameCellInfo.EDoorFlag.Up:
			case GameCellInfo.EDoorFlag.Down:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[0] : MapSprite.SpriteArray[9];
				break;
			case GameCellInfo.EDoorFlag.Left:
			case GameCellInfo.EDoorFlag.Right:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[1] : MapSprite.SpriteArray[10];
				break;
			case GameCellInfo.EDoorFlag.UL:
			case GameCellInfo.EDoorFlag.DR:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[2] : MapSprite.SpriteArray[11];
				break;
			case GameCellInfo.EDoorFlag.UD:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[3] : MapSprite.SpriteArray[12];
				break;
			case GameCellInfo.EDoorFlag.UR:
			case GameCellInfo.EDoorFlag.DL:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[4] : MapSprite.SpriteArray[13];
				break;
			case GameCellInfo.EDoorFlag.LR:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[5] : MapSprite.SpriteArray[14];
				break;
			case GameCellInfo.EDoorFlag.UDL:
			case GameCellInfo.EDoorFlag.UDR:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[6] : MapSprite.SpriteArray[15];
				break;
			case GameCellInfo.EDoorFlag.LRU:
			case GameCellInfo.EDoorFlag.LRD:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[7] : MapSprite.SpriteArray[16];
				break;
			case GameCellInfo.EDoorFlag.UDLR:
				spriteName = state == ELevelMapCellState.Around ? MapSprite.SpriteArray[8] : MapSprite.SpriteArray[17];
				break;
		}
		return spriteName;
	}
	
	public void Show()
	{
		MapCellObject.SetActive(true);
	}
}

public class LevelMap
{
	public GameObject MapObject;
	
	GameLevel mGameLevel;
	GameCell[] mGameCells;
	List<LevelMapCell>MapCells = new List<LevelMapCell>();
	Object mTitle;
	GameObject mMapPlayer;
	GameObject mEndFlag;
	LevelMapCell mBossCell;
	LevelMapCell mStartBridge;
	LevelMapCell mEndBridge;

	public void BuildMapPlayer()
	{
		mMapPlayer = GameObject.Instantiate(mTitle, Vector3.zero, Quaternion.identity) as GameObject;
		UISprite sp = mMapPlayer.GetComponent<UISprite>();
		sp.spriteName = "Button17_MiniMap_Player_01";
		GameObject go = new GameObject("MapPlayer");
		go.transform.parent = MapObject.transform.parent;
		go.transform.localPosition = Vector3.zero;
		mMapPlayer.transform.parent = go.transform;
		sp.MakePixelPerfect();
		Quaternion rotate = Quaternion.identity;
		rotate.eulerAngles = new Vector3(0, 0, 225);
		go.transform.localRotation = rotate;
		go.transform.localScale = Vector3.one;
		mMapPlayer.transform.localPosition = Vector3.zero;
		sp.depth = MapCells[0].MapCellObject.GetComponent<UISprite>().depth + 1;
	}
	
	public void BuildMapCell(GameCell[] gameCells, GameLevel gameLevel)
	{
		mGameLevel = gameLevel;
		mTitle = Resources.Load("MapTitle");
		mGameCells = gameCells;
		for(int i=0; i< gameCells.Length; i++)
		{
			LevelMapCell mapCell = new LevelMapCell();
			GameObject cellObj = GameObject.Instantiate(mTitle, Vector3.zero, Quaternion.identity) as GameObject;
			mapCell.MapCellObject = cellObj;
			Vector3 position = new Vector3(mGameCells[i].CellInfo.X*48, mGameCells[i].CellInfo.Y*40, 1);
			mapCell.Init(MapObject.transform, position, mGameCells[i].CellInfo.DoorMask, mGameCells[i].CellInfo.CellObject.transform.localRotation);

			InitMapCellState(i, mapCell);
			MapCells.Add(mapCell);
		}
		BuildMapPlayer();
		Quaternion rotate = Quaternion.identity;
		rotate.eulerAngles = new Vector3(0, 0, 225);
		MapObject.transform.localRotation = rotate;
		if((int)Global.CurSceneInfo.Type != 8)
			CreateFlag();
	}
	
	void InitMapCellState(int i, LevelMapCell mapCell)
	{
		if( i==0 )
		{
			mapCell.UpdateCellState(ELevelMapCellState.Around);
			// Build start brigde
			mStartBridge = BuildBridge(CalcBridgePosition(mGameCells[i], mGameLevel.StartBridge), mapCell);
			UISprite sp = mStartBridge.MapCellObject.GetComponent<UISprite>();
			sp.spriteName = "Button17_MiniMap_Start_02";
			sp.MakePixelPerfect();
		}
		else if( mGameCells[i].CellType == ECellType.Boss )
		{
			if((int)Global.CurSceneInfo.Type != 8)
				mapCell.UpdateCellState(ELevelMapCellState.Around);
			else
			{
				mapCell.UpdateCellState(ELevelMapCellState.None);
				mapCell.MapCellObject.SetActive(false);
			}
			mBossCell = mapCell;
			// Build end brigde
			mEndBridge = BuildBridge( CalcBridgePosition(mGameCells[i], mGameLevel.EndBridge), mapCell );
			mEndBridge.MapCellObject.GetComponent<UISprite>().spriteName = "Button17_MiniMap_Finish_01";
			UISprite sp = mEndBridge.MapCellObject.GetComponent<UISprite>();
			Quaternion rotate = Quaternion.identity;
			rotate.eulerAngles =  new Vector3(0, 0, 135);
			mEndBridge.MapCellObject.transform.localRotation = rotate;
			sp.MakePixelPerfect();
			mEndBridge.MapCellObject.SetActive(false);
		}
		else
		{
			mapCell.UpdateCellState(ELevelMapCellState.None);
			mapCell.MapCellObject.SetActive(false);
		}
	}
	
	void CreateFlag()
	{
		CreateCellFlag(mBossCell, "Button17_MiniMap_Boss_01");
		//CreateCellFlag(mStartBridge, "Button17_MiniMap_Start_02");
		//mEndFlag = CreateCellFlag(mEndBridge, "Button17_MiniMap_Finish_02");
	}
	
	GameObject CreateCellFlag(LevelMapCell mapCell, string spriteName)
	{
		GameObject flag = GameObject.Instantiate(mTitle, Vector3.zero, Quaternion.identity) as GameObject;
		flag.GetComponent<UISprite>().spriteName = spriteName;
		flag.transform.parent = MapObject.transform;
		flag.transform.localPosition = mapCell.MapCellObject.transform.localPosition;
		flag.transform.rotation = Quaternion.identity;
		flag.GetComponent<UISprite>().MakePixelPerfect();
		if(!mapCell.MapCellObject.activeSelf)
			flag.SetActive(false);
		return flag;
	}
	
	Vector3 CalcBridgePosition(GameCell cell, GameObject bridge)
	{
		Vector3 pos = new Vector3(0,0,0);
		if(bridge.transform.localPosition.x != cell.CellInfo.CellObject.transform.localPosition.x)
			pos.x = bridge.transform.localPosition.x > cell.CellInfo.CellObject.transform.localPosition.x ? 1 : -1;
		if(bridge.transform.localPosition.z != cell.CellInfo.CellObject.transform.localPosition.z)
			pos.y = bridge.transform.localPosition.z > cell.CellInfo.CellObject.transform.localPosition.z ? 1 : -1;
		pos.z = 1;
		return pos;
	}
	
	LevelMapCell BuildBridge(Vector3 pos, LevelMapCell mapCell)
	{
		LevelMapCell bridgeCell = new LevelMapCell();
		GameObject cellObj = GameObject.Instantiate(mTitle, Vector3.zero, Quaternion.identity) as GameObject;
		bridgeCell.MapCellObject = cellObj;

		float deltaX = pos.x * mapCell.MapCellObject.transform.localScale.x;
		float deltaY = pos.y * mapCell.MapCellObject.transform.localScale.y;
		bridgeCell.Init(MapObject.transform, 
			new Vector3(mapCell.MapCellObject.transform.localPosition.x + deltaX,
			mapCell.MapCellObject.transform.localPosition.y + deltaY, 
			1));

		return bridgeCell;
	}

	public void OnEnterCell(GameCell gameCell)
	{
		int index = FindGameCell(gameCell);
		if(index != -1 && MapCells[index].MapCellState == ELevelMapCellState.None)
		{
			MapCells[index].UpdateCellState(ELevelMapCellState.In);
		}
	}
	
	public void OnFinishCell(GameCell gameCell)
	{
		int index = FindGameCell(gameCell);
		if(index != -1  && MapCells[index].MapCellState != ELevelMapCellState.Boss)
		{
			MapCells[index].UpdateCellState(ELevelMapCellState.Pass);
			MapCells[index].Show();
		}
		if(gameCell.CellType == ECellType.Boss)
		{
			mEndBridge.MapCellObject.SetActive(true);
			//mEndFlag.SetActive(true);
		}
	}
	
	public void OnShowAroundCell(GameCell gameCell)
	{
		int index = FindGameCell(gameCell);
		if(index != -1 && MapCells[index].MapCellState != ELevelMapCellState.Pass && MapCells[index].MapCellState != ELevelMapCellState.Boss)
		{
			MapCells[index].UpdateCellState(ELevelMapCellState.Around);
			MapCells[index].Show();
		}
	}
	
	public void OnPlayerMove(Vector3 position)
	{
		if(mMapPlayer!=null)
		{
			float deltaX = (position.x - position.z) * 0.5f;
			float deltaY = (position.x + position.z) * 0.5f;
			
			// 12*8 ratio is 3*3 so 18*12 ratio is 12/18*3 8/12*3
			float ratioX = 12/mGameLevel.CellWidth * 3;
			float ratioY = 8 / mGameLevel.CellLength * 3;

			MapObject.transform.localPosition = new Vector3(deltaX*ratioX, deltaY*ratioY, 0);

			Quaternion rotation = UnitManager.Instance.LocalPlayer.UUnitInfo.gameObject.transform.localRotation;
			mMapPlayer.transform.localRotation = new Quaternion(rotation.x, rotation.z, -rotation.y, rotation.w);
		}
	}
		
	int FindGameCell(GameCell gameCell)
	{
		for(int i=0; i<MapCells.Count; i++)
		{
			if(mGameCells[i] == gameCell)
				return i;
		}
		return -1;
	}
}
