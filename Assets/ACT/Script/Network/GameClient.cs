using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using UnityEngine;

public class GameClient : SimpleClient
{
    int mPlayerId;
    int mGameId;
    int mPassWord;
    Unit mOwner;
    Unit mMaster;
    PlayerState mPlayerState = PlayerState.None;
    EGameState mGameState = EGameState.None;

    Dictionary<MessageId, List<OnResponse>> mResponseHandlers;
    Dictionary<UInt32, Unit> mPlayerMap;

    enum PlayerState
    {
        None = 0,
        Creating,
        Joing,
        InGame,
    };

    public enum EGameState
    {
        None = 0,
        Ready,
        Started,
        Finished,
    };

    public bool IsMaster { get { return mMaster == mOwner; } }
    public EGameState GameState { get { return mGameState; } }
    public int PlayerId { get { return mPlayerId; } }
    public Dictionary<UInt32, Unit> PlayerMap { get { return mPlayerMap; } }
    public GameStarted OnGameStarted { get; set; }
    public GameFinished OnGameFinished { get; set; }
    public UnitJoinGame OnUnitJoinGame { get; set; }
    public delegate void OnResponse(byte[] data);
    public delegate void GameStarted();
    public delegate void GameFinished(bool win);
    public delegate void UnitJoinGame(Unit unit);

    public bool HasPlayer(UInt32 id) { return mPlayerMap.ContainsKey(id); }
    public Unit GetPlayer(UInt32 id)
    {
        Unit ret = null;
        mPlayerMap.TryGetValue(id, out ret);
        return ret;
    }

    public GameClient(Unit owner, int playerId)
    {
        mOwner = owner;
        mOwner.ServerId = (uint)playerId;
        mPlayerId = playerId;
    }

    // Use this for initialization
    public void Start(string server, int gameId, int passWord)
    {
        mGameId = gameId;
        mPassWord = passWord;
        mPlayerMap = new Dictionary<uint, Unit>();
        mResponseHandlers = new Dictionary<MessageId, List<OnResponse>>();

        base.Start(server);

        RegisterHandler(MessageId.PlayAction, OnPlayActionCommand);
        RegisterHandler(MessageId.SyncPlayer, OnSyncPlayerCommand);
        RegisterHandler(MessageId.SyncMove, OnSyncMoveCommand);
        RegisterHandler(MessageId.SyncRotate, OnSyncRotateCommand);
        RegisterHandler(MessageId.HitData, OnHitDataCommand);
        RegisterHandler(MessageId.Dead, OnDeadCommand);
        RegisterHandler(MessageId.UpdateUserInfo, OnUpdateUserInfo);
        RegisterHandler(MessageId.MasterGameStated, OnMasterGameStated);
        RegisterHandler(MessageId.MasterGameFinished, OnMasterGameFinished);
        RegisterHandler(MessageId.Buff, OnBuff);
    }

    public void RegisterHandler(MessageId messageId, OnResponse handler)
    {
        if (!mResponseHandlers.ContainsKey(messageId))
            mResponseHandlers[messageId] = new List<OnResponse>();

        List<OnResponse> handlers = mResponseHandlers[messageId];
        if (!handlers.Contains(handler))
            handlers.Add(handler);
    }

    public void UnRegisterHandler(MessageId messageId, OnResponse handler)
    {
        List<OnResponse> handlers;
        if (!mResponseHandlers.TryGetValue(messageId, out handlers))
            return;

        handlers.Remove(handler);
    }

    public bool ProcessMessage(byte[] data)
    {
        if (data.Length < 2)
            return false;

        byte opCode = data[0];
        MessageId msgId = (MessageId)data[1];

        List<OnResponse> handlers;
        if (!mResponseHandlers.TryGetValue(msgId, out handlers))
            return false;

        // calling the message handler.
        foreach (OnResponse response in handlers)
            response(data);

        return true;
    }

    public override bool ProcessPacket(byte[] data)
    {
        if (data.Length < 1)
        {
            Debug.LogError("Got empty packet.");
            return false;
        }

        try
        {
            switch (data[0])
            {
                case NetCommon.SERVER_RESPONSE:
                    OnServerResponse(data);
                    break;
                case NetCommon.USER_JOIN_GAME:
                    OnUserJoinGame(data);
                    break;
                case NetCommon.USER_LEAVE_GAME:
                    OnUserLeaveGame(data);
                    break;
                case NetCommon.USER_MESSAGES:
                    return ProcessMessage(data);
                case NetCommon.SERVER_MASTER_CHANGE:
                    OnMasterChange(data);
                    break;
                default:
                    return false;
            }
        }
        catch (System.IO.EndOfStreamException ex)
        {
            Debug.LogError("Faile to process packet" + 
                " OPCode[0]:" + data[0] +
                " OPCode[1]:" + data[1] + 
                " Mgs:" + ex.Message);
            return false;
        }

        return true;
    }

    public override void OnOpend()
    {
        CreateGame();
    }

    public override void OnClosed()
    {
        if (mGameState == EGameState.Started)
        {
            MasterGameFinishedRequest command = new MasterGameFinishedRequest((UInt32)mPlayerId, false);
            SendCommand(command, true);
        }
    }

    public void CreateGame()
    {
        Debug.Log("CreateGame...");

        CreateGameRequest command = new CreateGameRequest(mGameId, mPassWord);
        SendCommand(command);

        mPlayerState = PlayerState.Creating;
    }

    public void JoinGame(bool isMaster)
    {
        JoinGameRequest command = new JoinGameRequest(mGameId, mPassWord, mPlayerId);
        SendCommand(command);

        mPlayerState = PlayerState.Joing;
        if (isMaster)
            mMaster = mOwner;

        Debug.Log("JoinGame...");
    }

    public Unit CreateUser(UInt32 userId)
    {
        if (Global.GPvpData == null || userId >= Global.GPvpData.users.Length)
            return null;

        PlayerData data = Global.GPvpData.users[(int)userId];
        Player player = Player.CreateShowPlayer(data, null);
        if (player == null)
            return null;

        // pvp mode we need check the hit.
        if (!player.UUnitInfo.initialization)
            UnitManager.Instance.Add(player);

        // assign server id.
        player.UpdateAttributes();
        player.ServerId = userId;
        mPlayerMap[userId] = player;

        if (OnUnitJoinGame != null)
            OnUnitJoinGame(player);

        return player;
    }

    public void MasterCheckGameFinished()
    {
        MasterCheckGameFinished(false);
    }

    class CampInfoTmp
    {
        public int LeftNum = 0;
        public int HpSum = 0;
    }

    public void MasterCheckGameFinished(bool forceEnd)
    {
        // only master can end this game.
        if (!IsMaster)
            return;

        // already finished.
        if (mGameState == EGameState.Finished)
            return;

        CampInfoTmp enemyInfo = new CampInfoTmp();
        CampInfoTmp friendInfo = new CampInfoTmp();
        foreach (Unit unit in mPlayerMap.Values)
        {
            int left = unit.Dead ? 0 : 1;
            int hpSum = unit.Dead ? 0 : unit.GetAttrib(EPA.CurHP) * 100 / unit.GetAttrib(EPA.HPMax);

            CampInfoTmp info = (unit.Camp == EUnitCamp.EUC_ENEMY) ? enemyInfo : friendInfo;
            info.LeftNum += left;
            info.HpSum += hpSum;
        }

        // here game continues.
        if (!forceEnd && enemyInfo.LeftNum > 0 && friendInfo.LeftNum > 0)
            return;
		
		Debug.Log("Master is ready to close the game.");
		
        bool win = false;
        if (enemyInfo.LeftNum != friendInfo.LeftNum)
            win = friendInfo.LeftNum > enemyInfo.LeftNum;
        else
            win = friendInfo.HpSum > enemyInfo.HpSum;

        // tell others we quit game.
        if (win)
        {
            MasterGameFinishedRequest command = new MasterGameFinishedRequest((UInt32)mPlayerId, true);
            SendCommand(command, true);
        }
        else
        {
            MasterGameFinishedRequest command = new MasterGameFinishedRequest((UInt32)mPlayerId, false);
            SendCommand(command, true);
        }
    }

    void OnUnitDead(Unit deadUnit)
    {
        if (IsMaster)
            MasterCheckGameFinished();
    }

    public void StartGame()
    {
        MasterGameStatedRequest command = new MasterGameStatedRequest((UInt32)mPlayerId);
        SendCommand(command, true);
    }

    void SyncPlayerInfo()
    {
        // send a sync player request.
        UInt32 UserID = (UInt32)PlayerId;
        Int16 X = 0;
        Int16 Y = 0;
        Int16 Z = 0;
        Int16 Dir = 0;
        byte Action = (byte)mOwner.ActionStatus.ActiveAction.ActionCache;
        NetCommon.Encode(mOwner.Position, mOwner.Orientation, ref X, ref Y, ref Z, ref Dir);

        SyncPlayerRequest command = new SyncPlayerRequest(
            UserID,
            X, Y, Z, Dir,
            Action,
            IsMaster ? (byte)1 : (byte)0);
        SendCommand(command);
    }

    #region "MessageHandler"
    void OnUserJoinGame(byte[] data)
    {
        JoinGameRequest command = new JoinGameRequest();
        command.Parse(data);

        Debug.Log("OnUserJoinGame: " + command.UserID);

        if (mPlayerMap.ContainsKey(command.UserID))
        {
            Debug.LogError("User with id: " + command.UserID + " alreay exist.");
            return;
        }

        CreateUser(command.UserID);

        SyncPlayerInfo();
    }

    void OnUserLeaveGame(byte[] data)
    {
        LeaveGameRequest command = new LeaveGameRequest();
        command.Parse(data);

        Debug.Log("OnUserLeaveGame: " + command.UserID);
        Unit user = GetPlayer(command.UserID);
        if (user == null)
        {
            Debug.LogError("User with id: " + command.UserID + " not found.");
            return;
        }

        // we should finish this game.
        if (mGameState == EGameState.Started)
        {
            MasterGameFinishedRequest cmd = new MasterGameFinishedRequest((UInt32)mPlayerId, true);
            SendCommand(cmd, true);
        }

        GameObject.Destroy(user.UUnitInfo.gameObject);

        mPlayerMap.Remove(command.UserID);
        if (IsMaster)
            MasterCheckGameFinished();
    }

    void OnServerResponse(byte[] data)
    {
        ServerResponse command = new ServerResponse();
        command.Parse(data);

        Debug.Log("OnServerResponse: " + command.Response);
        switch (command.Response)
        {
            case NetCommon.RESPONSE_GAME_ALREAY_EXIST:
                // if the game already exist, do join now.
                if (mPlayerState == PlayerState.Creating)
                {
                    Debug.Log("CreateGame failed, already exist, try join");
                    JoinGame(false);
                }
                break;
            case NetCommon.RESPONSE_OK:
                // if create game sucess, do join game now.
                if (mPlayerState == PlayerState.Creating)
                {
                    Debug.Log("CreateGame sucess, i am the master.");
                    JoinGame(true);
                }

                if (mPlayerState == PlayerState.Joing)
                {
                    Debug.Log("JoinGame sucess");
                    mPlayerMap[(uint)mPlayerId] = mOwner;

                    mPlayerState = PlayerState.InGame;
                    mGameState = EGameState.Ready;

                    SyncPlayerInfo();
                }
                break;
        }
    }

    void OnMasterChange(byte[] data)
    {
        MasterChangeResponse command = new MasterChangeResponse();
        command.Parse(data);

        Unit unit = GetPlayer(command.Master);
        if (unit == null)
        {
            Debug.LogError("The unit: " + command.Master + " doesnot exist. at OnMasterChange");
            return;
        }
		
		Debug.Log("OnMasterChange to: " + command.Master);
		
        mMaster = unit;
        if (IsMaster)
        {
            Debug.Log("I am the master now!!!");

            MasterCheckGameFinished();
        }
    }

    void OnPlayActionCommand(byte[] data)
    {
        PlayerActionRequest command = new PlayerActionRequest();
        command.Parse(data);

        //Debug.Log("OnPlayActionCommand: " + command.UserID + " Owner:" + mOwner.ServerId + " PlayerId:" + PlayerId);

        Unit unit = GetPlayer(command.UserID);
        if (unit == null)
        {
            Debug.LogError("User with id: " + command.UserID + " not found. at OnPlayActionCommand");
            return;
        }

        Vector3 pos = Vector3.zero;
        float rotate = 0;
        NetCommon.Decode(command.X, command.Y, command.Z, command.Dir, ref pos, ref rotate);

        unit.ClearSyncMove();
        unit.SetPosition(pos);
        unit.SetOrientation(rotate);
        unit.ActionStatus.ChangeAction((int)command.Action, 0);
    }

    void OnSyncPlayerCommand(byte[] data)
    {
        SyncPlayerRequest command = new SyncPlayerRequest();
        command.Parse(data);

        Unit unit = GetPlayer(command.UserID);
        if (unit == null)
            unit = CreateUser(command.UserID);

        Vector3 pos = Vector3.zero;
        float rotate = 0;
        NetCommon.Decode(command.X, command.Y, command.Z, command.Dir, ref pos, ref rotate);

        // here comes a master.
        if (command.Master == 1)
        {
            if (mMaster != null)
                Debug.LogError("Comes a master but already exist, replace the exist one!");
            else
                Debug.Log("Here comes a master: " + command.UserID);
            mMaster = unit;
        }

        unit.SetPosition(pos);
        unit.SetOrientation(rotate);
        unit.ActionStatus.ChangeAction((int)command.Action, 0);
    }

    void OnSyncMoveCommand(byte[] data)
    {
        PlayerSyncMoveRequest command = new PlayerSyncMoveRequest();
        command.Parse(data);

        Unit unit = GetPlayer(command.UserID);
        if (unit == null)
        {
            Debug.LogError("User with id: " + command.UserID + " not found. at: OnSyncMoveCommand");
            return;
        }

        Vector3 pos = Vector3.zero;
        float rotate = 0;
        NetCommon.Decode(command.X, 0, command.Z, 0, ref pos, ref rotate);
        unit.SetSyncMove(pos.x, pos.z, NetCommon.GPredictionTime, true);
    }

    void OnSyncRotateCommand(byte[] data)
    {
        PlayerSyncRotateRequest command = new PlayerSyncRotateRequest();
        command.Parse(data);

        Unit unit = GetPlayer(command.UserID);
        if (unit == null)
        {
            Debug.LogError("User with id: " + command.UserID + " not found. at: OnSyncRotateCommand");
            return;
        }

        float rotate = NetCommon.DecodeRotate(command.Rotate);
        unit.SetOrientation(rotate);
    }

    void OnHitDataCommand(byte[] data)
    {
        HitDataRequest command = new HitDataRequest();
        command.Parse(data);

        //Debug.Log("OnHitDataCommand: " + command.HitData.LashZ + " LashTime:" + command.HitData.LashTime);

        Unit unit = GetPlayer(command.HitData.Target);
        if (unit == null)
        {
            Debug.LogError("User with id: " + command.HitData.Target + " not found. at: OnSyncMoveCommand");
            return;
        }

        unit.OnHit(command.HitData, true);
    }

    void OnDeadCommand(byte[] data)
    {
        DeadRequest command = new DeadRequest();
        command.Parse(data);

        Unit unit = GetPlayer(command.Target);
        if (unit == null)
        {
            Debug.LogError("User with id: " + command.Target + " not found. at: OnSyncMoveCommand");
            return;
        }
		
		Debug.Log("Player: " + command.Target + " dead.");

        // sub hp until die!!!.
        while (!unit.Dead)
            unit.AddHp(-1000);

        OnUnitDead(unit);
    }

    void OnUpdateUserInfo(byte[] data)
    {
        UpdateUserAttribRequest command = new UpdateUserAttribRequest();
        command.Parse(data);

        Unit unit = GetPlayer(command.Target);
        if (unit == null)
        {
            Debug.LogError("User with id: " + command.Target + " not found. at: OnSyncMoveCommand");
            return;
        }

        // hp modifier.
        if (!unit.Dead)
        {
            unit.AddHp(command.HP - unit.GetAttrib(EPA.CurHP));
			unit.AddAbility(command.Ability - unit.GetAttrib(EPA.CurAbility));
            if (unit.Dead)
                OnUnitDead(unit);
        }

        // TODO: sync the ability & soul here.
    }

    void OnMasterGameStated(byte[] data)
    {
        mGameState = EGameState.Started;

        if (OnGameStarted != null)
            OnGameStarted();
    }

    void OnMasterGameFinished(byte[] data)
    {
        // turn the game state to finished.
        mGameState = EGameState.Finished;

        MasterGameFinishedRequest command = new MasterGameFinishedRequest();
        command.Parse(data);

        Unit master = GetPlayer(command.Master);
        if (master == null)
        {
            Debug.LogError("Master is not found. will use the old master but i am not sure it correct.");
            master = mMaster;
        }

        if (master == null)
        {
            Debug.LogError("No master in game");
            return;
        }

        bool sameWithMaster = (master.Camp == EUnitCamp.EUC_FRIEND);
        bool win = (sameWithMaster && (command.Win != 0)) || (!sameWithMaster && (command.Win == 0));
        Debug.Log("master.Camp: " + master.Camp);
        Debug.Log("command.Win: " + command.Win);
        Debug.Log("we about to quit game, win ? " + win);
		
        if (OnGameFinished != null)
            OnGameFinished(win);
    }

    void OnBuff(byte[] data)
    {
        BuffRequest command = new BuffRequest();
        command.Parse(data);

        Unit target = GetPlayer(command.Target);
        if (target != null)
            target.BuffManager.AddBuff(command.Id);
    }

    #endregion
}
