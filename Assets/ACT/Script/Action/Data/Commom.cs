using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Data;
using Data.Camera;

namespace Data
{
    public enum Operation
    {
        None = 0,
        Attack,
        SpAttack,
        Skill,
        Move,
        Jump,
        Grab,

        // 被动输入
        StandHit,           // 一般受击
        KnockOut,           // 击飞
        KnockBack,          // 击退
        KnockDown,          // 击倒
        DiagUp,             // 浮空
        Hold,               // 抓住
        AirHit,             // 浮空追击
        DownHit,            // 倒地追击
        FallDown,           // 跌倒
    }

    public enum RaceType
    {
        Enemy = 0,
        TeamMember,
        Self,
        Parent,
        Child,
    }

    public enum HitResultType
    {
        StandHit,           // 一般受击
        KnockOut,           // 击飞
        KnockBack,          // 击退
        KnockDown,          // 击倒
        DiagUp,             // 浮空
        Hold,               // 抓住
        AirHit,             // 浮空追击
        DownHit,            // 倒地追击
        FallDown,           // 跌倒
    }

    public enum DamageType
    {
        Normal,
        Special,
        Mix,
    }

    public enum WeaponType
    {
        None = 0,           // 无
        Metal,              // 金属
        Chain,              // 链状金属
        Stone,              // 石材
        Bone,               // 骨质
        Wood,               // 木质
        Flesh,              // 肉身
        Fluid,              // 流质肉体
        Gas,                // 气体
        Sword,              // 剑
        Broadsword,         // 刀
        Axe,                // 斧头
        Knife,              // 匕首
        Gun,                // 枪
    }

    public enum MaterialType
    {
        None = 0,           // 无
        Metal,              // 金属
        Chain,              // 链状金属
        Stone,              // 石材
        Bone,               // 骨质
        Wood,               // 木质
        Flesh,              // 肉身
        Fluid,              // 流质肉体
        Gas,                // 气体
    };

    public enum HitDefnitionFramType
    {
        CuboidType              = 0,  //长方体
        CylinderType            = 1,  //圆柱体
        RingType                = 2,  //圆环体
        SomatoType              = 3,  //受击体
        FanType                 = 4,  //扇形体
    };

    public enum EventType
    {
        None = 0,
        PlayEffect,         // 播放特效	
        PlayAnim,           // 播放动画	
        PlaySound,          // 播放音效	
        StatusOn,           // 打开状态
        StatusOff,          // 关闭状态
        LinkActionOn,       // 打开连接动作
        LinkActionOff,      // 关闭连接动作
        SetVelocity,        // 设定位移速度
        SetVelocity_X,      // 设定位移速度X
        SetVelocity_Y,      // 设定位移速度Y
        SetVelocity_Z,      // 设定位移速度Z
        SetDirection,       // 设定方向
        ExeScript,          // 执行脚本
        SetGravity,         // 设置重力
        SetHeightStatus,    // 设置高度状态
        SetActionStatus,    // 设置动作状态
        SetFragmentStatus,  // 设置片段状态
        AddUnit,            // 添加单位
        RemoveMyself,       // 自我毁灭
        DropItem,           // 掉落宝物
        SetColor,           // 设置颜色
        PickUp,             // 拾取
        CameraEffect,       // 摄像机
        ListTargets,        // 列举目标
        ClearTargets,       // 清除目标
        FaceTargets,        // 面向目标
        EnableWeaponTrail,  // 开启刀光
        DisableWeaponTrail, // 关闭刀光
        SetHitDefVelocity,  // 设置攻击定义的速度
        ClearHitDefs,       // 关闭所有的攻击定义
        SetVariable,        // 设置自定义变量
        AdjustVarible,      // 调整自定义变量
        GhostEffect,        // 幻影特效
        AttackTargets,      // 攻击选中的目标
        GoToTargets,        // 移动到目标
        SummonUnit,         // 召唤
        ControlUnit,        //控制召唤怪
        ActionLevel,        //动作等级
        RotateOnHit,        //受击转向
        HasCollision,       //产生碰撞
        Chat,
    }

    public enum HitDamageType
    {
        LightDamageType         = 0,            //轻击
        MediumDamageType        = 1,            //中击
        HeavyDamageType         = 2,            //重击
        KillShotDamageType      = 3,            //绝招
    }
    public enum InputType
    {
        Click = 0,          // 单击（键盘：按下瞬间）
        DoubleClick,        // 双击（键盘：双击瞬间）
        Press,              // 长按（键盘）
        Release,            // 松开（键盘：松开瞬间）
        Pressing,           // 按下（键盘的状态）
        Releasing,          // 松开（键盘的状态）
    }

    [Flags]
    public enum HeightStatusFlag
    {
        None = 0,
        Stand = 1,
        Ground = 2,
        LowAir = 4,
        HighAir = 8,
        Hold = 16,
    }

    public enum EActionState
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        Hit = 3,
        Defense = 4,
    };

    public enum ListTargetFrameType
    {
       Cuboid_ListType = 0,
       Fan_ListType = 1,
    }

    public enum ListTargetMode
    {
        MinDistance = 0,
        MinAngle,
        Random,
    }

    public class CommonAction
    {
        public const string Show = "S1000";//Show in CreatRole
		public const string ShowEquip = "S2000";//Show in Store And Info
		public const string Idle = "N0000"; // Idle in fight;
		public const string IdleInTown = "N1000";//Idle in town;
		public const string Run = "N0010"; //run in Fight;
		public const string RunInTown = "N1010";
		public const string Revive = "H0100";
		public const string Bounce = "H1042";
		public const string DieStand = "H1040";//stand Dying;
		public const string DieDown = "H1041"; // Lay Dying;
		public const string Knockout = "H0010"; //
		public const string KnockBack = "H0020";//
    }
}

