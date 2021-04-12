using UnityEngine;
using System.Collections;
using System;

public class PlayerListener : MonoBehaviour
{
    class LocalPlayerListener : IActionListener
    {
        PlayerListener mParent;
        float mLastActionTime = 0;
        float mLastSyncTime = 0;

        public LocalPlayerListener(PlayerListener parent)
        {
            mParent = parent;
        }

        public void Update(float deltaTime) { }

        public void OnActionChanging(Data.Action oldAction, Data.Action newAction)
        {
            if (oldAction == null || newAction == null)
                return;

            bool sendToServer = false;
            if (oldAction.NextActionCache != newAction.ActionCache)
            {
                // 仅仅播放普通动作[N]和攻击动作[W]，被动动作[H]需要冲击时间/速度等参数。
                if (newAction.ID[0] == 'N' ||
                    newAction.ID[0] == 'W')
                {
                    sendToServer = true;
                }
            }
            else
            {
                if (oldAction.ID[0] == 'H' &&
                    newAction.ID[0] == 'N')
                {
                    // 受到攻击之后缓过神来，告诉服务器你现在的位置。
                    sendToServer = true;
                }
                else if (oldAction.ID[0] == 'W')
                {
                    // 技能攻击完成后，告诉服务器你现在的位置。
                    sendToServer = true;
                }
                else if (oldAction.ID[0] == 'N' && newAction.ID[0] == 'N' &&
                    oldAction.CanMove && oldAction.ActionStatus == (int)Data.EActionState.Move &&
                    newAction.CanMove == false)
                {
                    sendToServer = true;
                }
            }

            if (sendToServer)
            {
                mLastActionTime = Time.timeSinceLevelLoad;
                mLastSyncTime = mLastActionTime;

                mParent.SendActionRequest(newAction);
            }
        }

        public void OnInputMove()
        {
            float currentTime = Time.timeSinceLevelLoad;
            bool needSync = currentTime > mLastSyncTime + NetCommon.SyncSinceSync;
            if (!needSync && (mLastSyncTime == mLastActionTime) && currentTime > mLastActionTime + NetCommon.SyncSinceAction)
                needSync = true;

            if (needSync)
            {
                mLastSyncTime = currentTime;
                mParent.SendSyncMoveRequest(NetCommon.GPredictionTime);
            }
        }

        public void OnHitData(HitData hitData)
        {
            mParent.SendHitData(hitData);
        }

        public void OnHurt(int damage) 
        {
            mParent.SendHurtData(damage);
        }

        public void OnBuff(UInt32 target, int id)
        {
            mParent.SendBuffData(target, id);
        }

        public void OnFaceTarget()
        {
            mParent.SendSyncRotateRquest();
        }

        public void OnAttribChanged(EPA attrib)
        {
            mParent.SyncUserAttribute();
        }
    }

    Unit mOwner;
    PvpClient mPvpClient;
    LocalPlayerListener mListener;

    // Use this for initialization
    void Start()
    {
        Invoke("Attach", 1.0f);
    }

    void Attach()
    {
        Debug.Log("Attach");

        mListener = new LocalPlayerListener(this);

        mOwner = gameObject.GetComponent<UnitInfo>().Unit;
        mOwner.ActionStatus.Bind(mListener);

        mPvpClient = gameObject.GetComponent<PvpClient>();
    }

    void SendActionRequest(Data.Action action)
    {
        UInt32 UserID = (UInt32)mPvpClient.GameClient.PlayerId;
        Int16 X = 0;
        Int16 Y = 0;
        Int16 Z = 0;
        Int16 Dir = 0;
        byte Action = (byte)action.ActionCache;
        NetCommon.Encode(mOwner.Position, mOwner.Orientation, ref X, ref Y, ref Z, ref Dir);

        PlayerActionRequest command = new PlayerActionRequest(UserID, X, Y, Z, Dir, Action);
        mPvpClient.GameClient.SendCommand(command);
    }

    void SendSyncMoveRequest(float predicateTime)
    {
        Vector3 ownerPos = mOwner.Position;
        float length = mOwner.ActionStatus.ActiveAction.MoveSpeed * 0.01f * predicateTime;
        Vector3 direction = new Vector3(Mathf.Sin(mOwner.Orientation), 0, Mathf.Cos(mOwner.Orientation));
        
        float backCheckOffset = 0.05f;
        Vector3 checkPos = new Vector3(mOwner.Position.x, mOwner.Position.y + mOwner.Radius, mOwner.Position.z) - direction * backCheckOffset;
        float checkLength = length + mOwner.Radius + backCheckOffset;

        RaycastHit hitInfo;
        if (Physics.Raycast(checkPos, direction, out hitInfo, checkLength))
        {
            float hitDistance = hitInfo.distance - mOwner.Radius;

            // player is blocked by the wall or something.
            if (hitDistance <= 0)
                return;

            length = hitDistance;
        }

        ownerPos.x += direction.x * length;
        ownerPos.z += direction.z * length;

        UInt32 UserID = (UInt32)mPvpClient.GameClient.PlayerId;
        Int16 X = 0;
        Int16 Y = 0;
        Int16 Z = 0;
        Int16 Dir = 0;
        NetCommon.Encode(ownerPos, mOwner.Orientation, ref X, ref Y, ref Z, ref Dir);

        PlayerSyncMoveRequest command = new PlayerSyncMoveRequest(UserID, X, Z);
        mPvpClient.GameClient.SendCommand(command);
    }

    void SendSyncRotateRquest()
    {
        UInt32 UserID = (UInt32)mPvpClient.GameClient.PlayerId;
        Int16 Dir = NetCommon.EncodeRotate(mOwner.Orientation);
        PlayerSyncRotateRequest command = new PlayerSyncRotateRequest(UserID, Dir);
        mPvpClient.GameClient.SendCommand(command);
    }

    void SendHitData(HitData hitData)
    {
        HitDataRequest command = new HitDataRequest(hitData);
        mPvpClient.GameClient.SendCommand(command);
    }

    void SendHurtData(int damage)
    {
        if (mOwner.Dead)
            SendDeadRequest();
        else
            SyncUserAttribute();
    }

    void SendDeadRequest()
    {
        UInt32 UserID = (UInt32)mPvpClient.GameClient.PlayerId;
        DeadRequest command = new DeadRequest(UserID);
        mPvpClient.GameClient.SendCommand(command, true);
    }

    void SyncUserAttribute()
    {
        UInt32 UserID = (UInt32)mPvpClient.GameClient.PlayerId;
        UpdateUserAttribRequest command = new UpdateUserAttribRequest(
            UserID, 
            mOwner.GetAttrib(EPA.CurHP),
            mOwner.GetAttrib(EPA.CurAbility),
            mOwner.GetAttrib(EPA.CurSoul));
        mPvpClient.GameClient.SendCommand(command);
    }

    void SendBuffData(UInt32 target, int id)
    {
        BuffRequest command = new BuffRequest(target, id);
        mPvpClient.GameClient.SendCommand(command);
    }
}
