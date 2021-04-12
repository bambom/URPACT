using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Action : ICloneable
    {
        public const String CATEGORY_ACTION = "动作";
        public const String CATEGORY_CONTROLLER = "控制器";
        public const String CATEGORY_BOUNDINGS = "包围盒";
        public const String CATEGORY_STATUS = "状态";

        private String mAnimationVersion = String.Empty;
        [Category(CATEGORY_ACTION)]
        [XmlIgnore]
        [Description("动作版本")]
        [DefaultValue("")]
        [ReadOnly(true)]
        public String AnimationVersion { get { return mAnimationVersion; } set { mAnimationVersion = value; } }

        private String mActionVersion = String.Empty;
        [Category(CATEGORY_ACTION)]
        [Description("动作版本")]
        [ReadOnly(true)]
        [DefaultValue("")]
        [XmlIgnore]
        public String ActionVersion { get { return mActionVersion; } set { mActionVersion = value; } }

        private String mGfxVersion = String.Empty;
        [Category(CATEGORY_ACTION)]
        [Description("特效版本")]
        [DefaultValue("")]
        [ReadOnly(true)]
        [XmlIgnore]
        public String GfxVersion { get { return mGfxVersion; } set { mGfxVersion = value; } }


        #region CATEGORY_ACTION
        String mID = "N0000";
        [Category(CATEGORY_ACTION)]
        [Description("动作ID")]
        [DisplayName("动作ID")]
        public String ID { get { return mID; } set { mID = value; } }

        String mName = "未命名";
        [Category(CATEGORY_ACTION)]
        [Description("动作名称")]
        [DisplayName("动作名称")]
        public String Name { get { return mName; } set { mName = value; } }

        String mDesc = "动作";
        [Category(CATEGORY_ACTION)]
        [Description("动作的描述")]
        [DisplayName("描述")]
        public String Desc { get { return mDesc; } set { mDesc = value; } }

        String mDefaultLinkActionID;
        [Category(CATEGORY_ACTION)]
        [Description("缺省连接的动作ID号")]
        [DisplayName("缺省连接动作")]
        public String DefaultLinkActionID { get { return mDefaultLinkActionID; } set { mDefaultLinkActionID = value; } }

        List<AnimSlot> mAnimSlotList = new List<AnimSlot>();
        [Category(CATEGORY_ACTION)]
        [DisplayName("动画列表")]
        public List<AnimSlot> AnimSlotList { get { return mAnimSlotList; } }

        int mAnimTime = 1000;
        [Category(CATEGORY_ACTION)]
        [Description("动画的时间(单位毫秒)")]
        [DisplayName("动画的时间")]
        [DefaultValue(1000)]
        public int AnimTime { get { return mAnimTime; } set { mAnimTime = value; } }

        int mBlendTime = 100;
        [Category(CATEGORY_ACTION)]
        [Description("动画转换到其他动画时候的混合时间，默认为100(单位毫秒)")]
        [DisplayName("动画混合时间")]
        [DefaultValue(100)]
        public int BlendTime { get { return mBlendTime; } set { mBlendTime = value; } }

        int mMoveSpeed = 0;
        [Category(CATEGORY_ACTION)]
        [Description("1秒里面动作移动的速度(单位厘米)")]
        [DisplayName("移动速度")]
        [DefaultValue(0)]
        public int MoveSpeed { get { return mMoveSpeed; } set { mMoveSpeed = value; } }

        int mPoseTime = 0;
        [Category(CATEGORY_ACTION)]
        [Description("动画播放完成之后Blend到POSE的时间(单位毫秒)")]
        [DisplayName("POSE的时间")]
        [DefaultValue(0)]
        public int PoseTime { get { return mPoseTime; } set { mPoseTime = value; } }

        [Category(CATEGORY_ACTION)]
        [Description("动作的总时间为[AnimTime+PoseTime](单位毫秒)")]
        [DisplayName("动作的总时间")]
        [DefaultValue(0)]
        public int TotalTime { get { return mAnimTime + mPoseTime; } }

        public int mBoundingLengthRadio = 100;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("包围盒的长度比例调整")]
        [DisplayName("长度比例")]
        [DefaultValue(100)]
        public int BoundingLengthRadio { get { return mBoundingLengthRadio; } set { mBoundingLengthRadio = value; } }

        public int mBoundingWidthRadio = 100;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("包围盒的宽度比例调整")]
        [DisplayName("宽度比例")]
        [DefaultValue(100)]
        public int BoundingWidthRadio { get { return mBoundingWidthRadio; } set { mBoundingWidthRadio = value; } }

        public int mBoundingHeightRadio = 100;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("包围盒的高度比例调整")]
        [DisplayName("高度比例")]
        [DefaultValue(100)]
        public int BoundingHeightRadio { get { return mBoundingHeightRadio; } set { mBoundingHeightRadio = value; } }

        int mBoundingOffsetX = 0;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("包围盒的偏移，单位为厘米")]
        [DisplayName("包围盒X偏移")]
        [DefaultValue(0)]
        public int BoundingOffsetX { get { return mBoundingOffsetX; } set { mBoundingOffsetX = value; } }

        int mBoundingOffsetY = 0;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("包围盒的偏移，单位为厘米")]
        [DisplayName("包围盒Y偏移")]
        [DefaultValue(0)]
        public int BoundingOffsetY { get { return mBoundingOffsetY; } set { mBoundingOffsetY = value; } }

        int mBoundingOffsetZ = 0;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("包围盒的偏移，单位为厘米")]
        [DisplayName("包围盒Z偏移")]
        [DefaultValue(0)]
        public int BoundingOffsetZ { get { return mBoundingOffsetZ; } set { mBoundingOffsetZ = value; } }

        int mCollsionLengthRadio = 100;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("碰撞框的长度比例调整")]
        [DisplayName("长度比例")]
        [DefaultValue(100)]
        public int CollisionLengthRadio { get { return mCollsionLengthRadio; } set { mCollsionLengthRadio = value; } }

        int mCollsionWidthRadio = 100;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("碰撞框的宽度比例调整")]
        [DisplayName("宽度比例")]
        [DefaultValue(100)]
        public int CollisionWidthRadio { get { return mCollsionWidthRadio; } set { mCollsionWidthRadio = value; } }

        int mCollsionHeightRadio = 100;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("碰撞框的高度比例调整")]
        [DisplayName("高度比例")]
        [DefaultValue(100)]
        public int CollisionHeightRadio { get { return mCollsionHeightRadio; } set { mCollsionHeightRadio = value; } }

        int mCollsionOffsetX = 0;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("碰撞框的偏移，单位为厘米")]
        [DisplayName("碰撞框X偏移")]
        [DefaultValue(0)]
        public int CollisionOffsetX { get { return mCollsionOffsetX; } set { mCollsionOffsetX = value; } }

        int mCollsionOffsetY = 0;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("碰撞框的偏移，单位为厘米")]
        [DisplayName("碰撞框Y偏移")]
        [DefaultValue(0)]
        public int CollisionOffsetY { get { return mCollsionOffsetY; } set { mCollsionOffsetY = value; } }

        int mCollsionOffsetZ = 0;
        [Category(CATEGORY_BOUNDINGS)]
        [Description("碰撞框的偏移，单位为厘米")]
        [DisplayName("碰撞框Z偏移")]
        [DefaultValue(0)]
        public int CollisionOffsetZ { get { return mCollsionOffsetZ; } set { mCollsionOffsetZ = value; } }

        public int mHeightStatus = 0;
        [Category(CATEGORY_STATUS)]
        [Description("站立=0，地面=1，低空=2，高空=3, 被抓=4")]
        [DisplayName("高度状态")]
        [DefaultValue(0)]
        public int HeightStatus { get { return mHeightStatus; } set { mHeightStatus = value; } }

        public int mActionStatus = 0;
        [Category(CATEGORY_STATUS)]
        [Description("待机=0，移动=1，攻击=2，受伤=3，防御=4")]
        [DisplayName("动作状态")]
        [DefaultValue(0)]
        public int ActionStatus { get { return mActionStatus; } set { mActionStatus = value; } }

        public int mFragmentStatus = 1;
        [Category(CATEGORY_STATUS)]
        [Description("准备=0，行动=1，回复=2，硬直=3")]
        [DisplayName("片段状态")]
        [DefaultValue(1)]
        public int FragmentStatus { get { return mFragmentStatus; } set { mFragmentStatus = value; } }

        public int mActionLevel = 0;
        [Description("动作等级为0的时候，该动作采用单位的默认动作，否则为指定的设定等级。默认值为0")]
        [DisplayName("动作等级")]
        [DefaultValue(0)]
        public int ActionLevel { get { return mActionLevel; } set { mActionLevel = value; } }

        private bool mResetVelocity = true;
        [DefaultValue(true), DisplayName("是否重置速度"), XmlAttribute("ResetVelocity")]
        public bool ResetVelocity { get { return mResetVelocity; } set { mResetVelocity = value; } }

        public int mHasCollision = 0;
        [DefaultValue(0), DisplayName("是否生成碰撞"), XmlAttribute("HasCollision")]
        public int HasCollision { get { return mHasCollision; } set { mHasCollision = value; } }

        public int mRotateOnHit = 0;
        [DefaultValue(0), DisplayName("受击后转向"), XmlAttribute("RotateOnHit")]
        public int RotateOnHit { get { return mRotateOnHit; } set { mRotateOnHit = value; } }


        public List<Event> mEvents = new List<Event>();
        [Description("该动作内能够触发的事件列表")]
        [DisplayName("内部事件")]
        public List<Event> Events { get { return mEvents; } }

        public List<ActionInterrupt> mActionInterrupts = new List<ActionInterrupt>();
        [Description("该动作内能够被中断的动作")]
        [DisplayName("中断列表")]
        public List<ActionInterrupt> ActionInterrupts { get { return mActionInterrupts; } }

        public List<AttackDef> mAttackDefs = new List<AttackDef>();
        [Description("该动作所包含的攻击定义列表")]
        [DisplayName("攻击定义")]
        public List<AttackDef> AttackDefs { get { return mAttackDefs; } }

        #endregion

        public int mActionCache = 0;
        [DefaultValue(0), XmlAttribute("ActionCache")]
        public int ActionCache { get { return mActionCache; } set { mActionCache = value; } }

        public int mNextActionCache = 0;
        [DefaultValue(0), XmlAttribute("NextActionCache")]
        public int NextActionCache { get { return mNextActionCache; } set { mNextActionCache = value; } }

        #region CATEGORY_CONTROLLER
        public bool mIgnoreGravity;
        [Category(CATEGORY_CONTROLLER)]
        [Description("切换玩家状态到：空中")]
        [DisplayName("空中")]
        [DefaultValue(false)]
        public bool IgnoreGravity { get { return mIgnoreGravity; } set { mIgnoreGravity = value; } }

        public bool InSky { set { mIgnoreGravity = value; } }
        
        bool mFaceTarget;
        [Category(CATEGORY_CONTROLLER)]
        [Description("面向目标")]
        [DisplayName("面向目标")]
        [DefaultValue(false)]
        public bool FaceTarget { get { return mFaceTarget; } set { mFaceTarget = value; } }

        public bool mCanMove;
        [Category(CATEGORY_CONTROLLER)]
        [Description("切换玩家状态到：可移动")]
        [DisplayName("可移动")]
        [DefaultValue(false)]
        public bool CanMove { get { return mCanMove; } set { mCanMove = value; } }

        public bool mCanRotate;
        [Category(CATEGORY_CONTROLLER)]
        [Description("切换玩家状态到：可转身")]
        [DisplayName("可转身")]
        [DefaultValue(false)]
        public bool CanRotate { get { return mCanRotate; } set { mCanRotate = value; } }

        public bool mCanHurt;
        [Category(CATEGORY_CONTROLLER)]
        [Description("切换玩家状态到：可受伤")]
        [DisplayName("可受伤")]
        [DefaultValue(false)]
        public bool CanHurt { get { return mCanHurt; } set { mCanHurt = value; } }

        bool mIsGOD;
        [Category(CATEGORY_CONTROLLER)]
        [Description("切换玩家状态到：无敌")]
        [DisplayName("无敌")]
        [DefaultValue(false)]
        public bool IsGOD { get { return mIsGOD; } set { mIsGOD = value; } }

        private bool mCameraTarget = false;
        [DefaultValue(false), DisplayName("摄像机视角"), XmlAttribute("CameraTarget")]
        public bool CameraTarget { get { return mCameraTarget; } set { mCameraTarget = value; } }

        int mEasyDiffTimeProp = 100;
        [Category(CATEGORY_CONTROLLER)]
        [Description("简单难度下时间比列")]
        [DisplayName("简单难度下时间比列")]
        [DefaultValue(100)]
        public int EasyDiffTimeProp { get { return mEasyDiffTimeProp; } set { mEasyDiffTimeProp = value; } }

        int mNomalDiffTimeProp = 100;
        [Category(CATEGORY_CONTROLLER)]
        [Description("普通难度下时间比列")]
        [DisplayName("普通难度下时间比列")]
        [DefaultValue(100)]
        public int NomalDiffTimeProp { get { return mNomalDiffTimeProp; } set { mNomalDiffTimeProp = value; } }

        int mHardDiffTimeProp = 100;
        [Category(CATEGORY_CONTROLLER)]
        [Description("困难难度下时间比列")]
        [DisplayName("困难难度下时间比列")]
        [DefaultValue(100)]
        public int HardDiffTimeProp { get { return mHardDiffTimeProp; } set { mHardDiffTimeProp = value; } }

        int mNightmareDiffTimeProp = 100;
        [Category(CATEGORY_CONTROLLER)]
        [Description("噩梦难度下时间比列")]
        [DisplayName("噩梦难度下时间比列")]
        [DefaultValue(100)]
        public int NightmareDiffTimeProp { get { return mNightmareDiffTimeProp; } set { mNightmareDiffTimeProp = value; } }

        #endregion

        public class ActionCompare : Comparer<Action>
        {
            public override int Compare(Action x, Action y)
            {
                return x.ID.CompareTo(y.ID);
            }
        }

        public System.Object Clone()
        {
            // save current object to xml.
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Action));
            serializer.Serialize(memoryStream, this);

            // deserializer out.
            memoryStream.Seek(0, SeekOrigin.Begin);
            Action newAction = (Action)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            // reset copy datas.
            return newAction;
        }

        public override string ToString()
        {
            if (Desc.Length == 0)
                return ID + "(" + Name + ")";
            else
                return ID + "(" + Name + ")[" + Desc + "]";
        }

        public void ClearEffectData()
        {
            // clear the events effects data.
            List<Event> deleteEvents = new List<Event>();
            foreach (Event actionEvent in Events)
            {
                if (actionEvent.EventType == EventType.PlayEffect )
                    deleteEvents.Add(actionEvent);
            }

            foreach (Event deleteEvent in deleteEvents)
                Events.Remove(deleteEvent);

            // clear the 
            foreach (AttackDef attackDef in AttackDefs)
            {
                attackDef.SelfEffect = "";
                attackDef.HitedEffect = "";
            }
        }

        public void ClearSoundData()
        {
            // clear the events effects data.
            List<Event> deleteEvents = new List<Event>();
            foreach (Event actionEvent in Events)
            {
                if (actionEvent.EventType == EventType.PlaySound)
                    deleteEvents.Add(actionEvent);
            }

            foreach (Event deleteEvent in deleteEvents)
                Events.Remove(deleteEvent);

            // clear the 
            foreach (AttackDef attackDef in AttackDefs)
            {
                attackDef.SelfSound = "";
                attackDef.HitedSound = "";
            }
        }

        public void BuildActionCache(ActionGroup group)
        {
            mActionCache = group.GetActionIdx(ID);
            mNextActionCache = group.GetActionIdx(mDefaultLinkActionID);
            foreach (ActionInterrupt interrupt in mActionInterrupts)
            {
                interrupt.ActionCache = group.GetActionIdx(interrupt.ActionID);

                // check additional action.
                if (interrupt.DetectVariable && interrupt.CompareValue == 0)
                {
                    if (interrupt.Variable == (int)EVariableIdx.EVI_HP ||
                        interrupt.Variable == (int)EVariableIdx.EVI_HPPercent)
                    {
                        Debug.LogWarning(
                            string.Format("Action shouldnot check variable with 0, use IsDead instead!!! [{0}][{1}]", group.Race, ID));
                    }
                }
            }
        }
    }
}
