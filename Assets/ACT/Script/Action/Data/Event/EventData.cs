using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    /*[XmlInclude(typeof(EventPlayAnim))]
    [XmlInclude(typeof(EventRemoveMyself))]
    [XmlInclude(typeof(EventDropItem))]
    [XmlInclude(typeof(EventPickUp))]
    [XmlInclude(typeof(EventCameraEffect))]
    [XmlInclude(typeof(EventClearTargets))]
    [XmlInclude(typeof(EventEnableWeaponTrail))]
    [XmlInclude(typeof(EventDisableWeaponTrail))]
    [XmlInclude(typeof(EventSetHitDefVelocity))]
    [XmlInclude(typeof(EventClearHitDefs))]
    [XmlInclude(typeof(EventGhostEffect))]*/
    [Serializable]
    [XmlInclude(typeof(EventPlayEffect))]
    [XmlInclude(typeof(EventSetColor))]
    [XmlInclude(typeof(EventLinkActionOn))]
    [XmlInclude(typeof(EventLinkActionOff))]
    [XmlInclude(typeof(EventSetVelocity))]
    [XmlInclude(typeof(EventSetDirection))]
    [XmlInclude(typeof(EventSetGravity))]
    [XmlInclude(typeof(EventSetHeightStatus))]
    [XmlInclude(typeof(EventSetActionStatus))]
    [XmlInclude(typeof(EventSetFragmentStatus))]
    [XmlInclude(typeof(EventAddUnit))]
    [XmlInclude(typeof(EventListTargets))]
    [XmlInclude(typeof(EventSetVariable))]
    [XmlInclude(typeof(EventAdjustVarible))]
    [XmlInclude(typeof(EventAttackTargets))]
    [XmlInclude(typeof(EventGoToTargets))]
    [XmlInclude(typeof(EventSummonUnit))]
    [XmlInclude(typeof(EventControlUnit))]
    [XmlInclude(typeof(EventActionLevel))]
    [XmlInclude(typeof(EventRotateOnHit))]
    [XmlInclude(typeof(EventHasCollision))]
    [XmlInclude(typeof(EventPlaySound))]
    [XmlInclude(typeof(EventStatusOn))]
    [XmlInclude(typeof(EventStatusOff))]
    [XmlInclude(typeof(EventSetVelocity_X))]
    [XmlInclude(typeof(EventSetVelocity_Y))]
    [XmlInclude(typeof(EventSetVelocity_Z))]
    [XmlInclude(typeof(EventExeScript))]
    [XmlInclude(typeof(EventCameraEffect))]
    [XmlInclude(typeof(EventChat))]
    [XmlInclude(typeof(EventFaceTargets))]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public abstract class EventData
    {
        public abstract EventType Type();
        public abstract String EventContent();

    }
}
