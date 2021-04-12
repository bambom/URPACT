using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class Event
    {
        public int mTriggerTime = 0;
        [Description("事件触发的时间点(百分比)，数值为[0-100-200]，[0-100]期间是处于动画时间段里面的百分比，[100-200]期间是出于POSE时间段里面的百分比")]
        [DisplayName("触发时间")]
        [DefaultValue(0)]
        public int TriggerTime { get { return mTriggerTime; } set { mTriggerTime = value; } }

        EventType mEventType = EventType.None;
        [Description("事件触发的类型，默认为None")]
        [DisplayName("触发类型")]
        [DefaultValue(EventType.None)]
        public EventType EventType 
        { 
            get { return mEventType; } 
            set 
            { 
                mEventType = value;
                mEventDetailData = null;
                switch (value)
                {
                    case EventType.ActionLevel:
                        mEventDetailData = new EventActionLevel();
                        break;
                    case EventType.AddUnit:
                        mEventDetailData = new EventAddUnit();
                        break;
                    case EventType.AdjustVarible:
                        mEventDetailData = new EventAdjustVarible();
                        break;
                    case EventType.AttackTargets:
                        mEventDetailData = new EventAttackTargets();
                        break;
                    case EventType.ControlUnit:
                        mEventDetailData = new EventControlUnit();
                        break;
                    case EventType.GoToTargets:
                        mEventDetailData = new EventGoToTargets();
                        break;
                    case EventType.HasCollision:
                        mEventDetailData = new EventHasCollision();
                        break;
                    case EventType.LinkActionOff:
                        mEventDetailData = new EventLinkActionOff();
                        break;
                    case EventType.LinkActionOn:
                        mEventDetailData = new EventLinkActionOn();
                        break;
                    case EventType.ListTargets:
                        mEventDetailData = new EventListTargets();
                        break;
                    case EventType.PlayEffect:
                        mEventDetailData = new EventPlayEffect();
                        break;
                    case EventType.RotateOnHit:
                        mEventDetailData = new EventRotateOnHit();
                        break;
                    case EventType.SetActionStatus:
                        mEventDetailData = new EventSetActionStatus();
                        break;
                    case EventType.SetColor:
                        mEventDetailData = new EventSetColor();
                        break;
                    case EventType.SetDirection:
                        mEventDetailData = new EventSetDirection();
                        break;
                    case EventType.SetFragmentStatus:
                        mEventDetailData = new EventSetFragmentStatus();
                        break;
                    case EventType.SetGravity:
                        mEventDetailData = new EventSetGravity();
                        break;
                    case EventType.SetHeightStatus:
                        mEventDetailData = new EventSetHeightStatus();
                        break;
                    case EventType.SetVariable:
                        mEventDetailData = new EventSetVariable();
                        break;
                    case EventType.SetVelocity:
                        mEventDetailData = new EventSetVelocity();
                        break;
                    case EventType.SummonUnit:
                        mEventDetailData = new EventSummonUnit();
                        break;
                    case EventType.PlaySound:
                        mEventDetailData = new EventPlaySound();
                        break;
                    case EventType.StatusOn:
                        mEventDetailData = new EventStatusOn();
                        break;
                    case EventType.StatusOff:
                        mEventDetailData = new EventStatusOff();
                        break;
                    case EventType.SetVelocity_X:
                        mEventDetailData = new EventSetVelocity_X();
                        break;
                    case EventType.SetVelocity_Y:
                        mEventDetailData = new EventSetVelocity_Y();
                        break;
                    case EventType.SetVelocity_Z:
                        mEventDetailData = new EventSetVelocity_Z();
                        break;
                    case EventType.ExeScript:
                        mEventDetailData = new EventExeScript();
                        break;
                    case EventType.CameraEffect:
                        mEventDetailData = new EventCameraEffect();
                        break;
                    case EventType.Chat:
                        mEventDetailData = new EventChat();
                        break;
                    case EventType.FaceTargets:
                        mEventDetailData = new EventFaceTargets();
                        break;
                }
            } 
        }

        [Browsable(false)]
        [DefaultValue((int)EventType.None)]
        [Description("EventType的Int数值，方便C++的XML读取")]
        public int EventTypeInt { get { return (int)mEventType; } set { } }

        EventData mEventDetailData = null;
        [Description("事件详细数据")]
        [DisplayName("事件详细数据")]
        public EventData EventDetailData { get { return mEventDetailData; } set { mEventDetailData = value; } }

        String mDesc = "";
        [Description("事件的描述")]
        [DisplayName("描述")]
        [DefaultValue("")]
        public String Desc { get { return mDesc; } set { mDesc = value; } }

        public override string ToString()
        {
            return EventType.ToString();
        }
    }
}
