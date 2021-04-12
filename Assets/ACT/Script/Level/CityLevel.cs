using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityLevel : MonoBehaviour
{
    static CityLevel msInstance;
    public static CityLevel Instance { get { return msInstance; } }

    Dictionary<string, Player> mOtherPlayers = new Dictionary<string, Player>();
    Vector3 mInitPosition = Vector3.zero;
    Vector3 mLastSyncPos = Vector3.zero;
    float mLastSyncTime = 0;
    const float SyncTime = 5.0f;
    const float RandomRange = 5.0f;

    // Use this for initialization
    void Start()
    {
        msInstance = this;
        if (MainScript.Instance != null)
        {
            MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.EnterCity, OnEnterCity);
            MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.LeaveCity, OnLeaveCity);
            MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.MoveInCity, OnMoveInCity);
        }

        if (!InGameMainWnd.Exist)
            InGameMainWnd.Instance.Open();

        if (UnitManager.Instance.LocalPlayer != null)
        {
            mInitPosition = UnitManager.Instance.LocalPlayer.Position;

            Vector3 radomPos = mInitPosition + new Vector3(Random.Range(-RandomRange, RandomRange), 0, Random.Range(-RandomRange, RandomRange));
            UnityEngine.AI.NavMeshHit navMeshHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(radomPos, out navMeshHit, RandomRange * 2, 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Default")))
                UnitManager.Instance.LocalPlayer.SetPosition(navMeshHit.position + Vector3.up * 0.2f);
            UnitManager.Instance.LocalPlayer.PlayAction(Data.CommonAction.IdleInTown);
        }

        FetchActiveUsers();
    }

    void OnDestroy()
    {
        msInstance = null;
        if (MainScript.Instance != null)
        {
            MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.EnterCity, OnEnterCity);
            MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.LeaveCity, OnLeaveCity);
            MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.MoveInCity, OnMoveInCity);
        }
    }

    void OnEnterCity(string err, Response response)
    {
        CityPlayerInfo playerInfo = (response != null) ? response.Parse<CityPlayerInfo>() : null;
        if (playerInfo != null)
            InstantiatePlayer(playerInfo);
    }

    void FetchActiveUsers()
    {
        StartCoroutine(MainScript.Execute(new GetCityUsersCmd(), delegate(string err, Response response)
        {
            CityUsers playerInfos = (response != null) ? response.Parse<CityUsers>() : null;
            if (playerInfos == null || playerInfos.users == null)
                return;

            List<string> leavedUsers = new List<string>(mOtherPlayers.Keys);
            foreach (CityPlayerInfo playerInfo in playerInfos.users)
            {
                if (playerInfo.data == null || 
                    playerInfo.data.Attrib == null ||
                    string.IsNullOrEmpty(playerInfo.data.Attrib.Name))
                    continue;

                if (leavedUsers.Contains(playerInfo.data.Attrib.Name))
                    leavedUsers.Remove(playerInfo.data.Attrib.Name);
                InstantiatePlayer(playerInfo);
            }

            // remove the leaved users.
            foreach (string leavedUser in leavedUsers)
            {
                Player player;
                if (!mOtherPlayers.TryGetValue(leavedUser, out player))
                    continue;

                player.Destory();
                mOtherPlayers.Remove(leavedUser);
            }
        }));
    }

    void InstantiatePlayer(CityPlayerInfo playerInfo)
    {
        if (playerInfo.data == null)
            return;

        Player player = InstantiatePlayer(playerInfo.data);
        if (player != null && playerInfo.pos != null)
        {
            Vector3 pos = new Vector3(playerInfo.pos.x, playerInfo.pos.y, playerInfo.pos.z);
            if (pos == Vector3.zero)
            {
                // sample a valid position for init player.
                Vector3 radomPos = mInitPosition + new Vector3(Random.Range(-RandomRange, RandomRange), 0, Random.Range(-RandomRange, RandomRange));
                UnityEngine.AI.NavMeshHit navMeshHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(radomPos, out navMeshHit, RandomRange * 2, 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Default")))
                    pos = navMeshHit.position;
                else
                    pos = mInitPosition;
            }
            player.SetPosition(pos);
        }
    }

    Player InstantiatePlayer(PlayerData data)
    {
        if (mOtherPlayers.ContainsKey(data.Attrib.Name))
            return mOtherPlayers[data.Attrib.Name];
		
		Player player = Player.CreateShowPlayer(data, null);
		if (player == null)
			return null;
		
        mOtherPlayers[data.Attrib.Name] = player;
        player.PlayAction(Data.CommonAction.IdleInTown);
        return player;
    }

    void OnLeaveCity(string err, Response response)
    {
        string user = (response != null) ? response.data : null;
        if (string.IsNullOrEmpty(user))
            return;

        Player player;
        if (!mOtherPlayers.TryGetValue(user, out player))
        {
            Debug.LogError(string.Format("Unit [{0}] does not exist in scene.", user));
            return;
        }

        player.Destory();

        mOtherPlayers.Remove(user);
    }

    void OnMoveInCity(string err, Response response)
    {
        MoveInfo moveInfo = (response != null) ? response.Parse<MoveInfo>() : null;
        if (moveInfo == null)
            return;

        Player player;
        if (!mOtherPlayers.TryGetValue(moveInfo.user, out player))
        {
            Debug.LogError(string.Format("Unit [{0}] does not exist in scene.", moveInfo.user));
            return;
        }

        // set setup position simply.
        Vector3 pos = new Vector3((float)moveInfo.pos.x, (float)moveInfo.pos.y, (float)moveInfo.pos.z);
        if (pos != Vector3.zero)
            player.StartNavigation(pos);
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerMoving();
    }

    void CheckPlayerMoving()
    {
        // check the position if player does not moving.
        Unit player = UnitManager.Instance.LocalPlayer;
        if (player == null || !player.UUnitInfo || player.ActionStatus.CanMove)
            return;

        float deltaTime = (Time.timeSinceLevelLoad - mLastSyncTime);
        if (deltaTime > SyncTime)
        {
            Vector3 position = player.UGameObject.transform.position;
            Vector3 transfer = position - mLastSyncPos;
            if (transfer.sqrMagnitude > 0.01f)
            {
                mLastSyncTime = Time.timeSinceLevelLoad;
                mLastSyncPos = position;

                MoveCmd request = new MoveCmd(position.x, position.y, position.z);
                StartCoroutine(MainScript.Execute(request, null));
            }
        }
    }

    public void OnReconnect()
    {
        FetchActiveUsers();

        // refresh the chat message.
        Chat.Instance.RefreshChatMessages();
    }
}
