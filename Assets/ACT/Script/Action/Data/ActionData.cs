using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.Camera;

namespace Data
{
    [Serializable]
    public class ActionData
    {
        private List<UnitActionInfo> objectTypes = new List<UnitActionInfo>();
        public List<UnitActionInfo> ObjectTypes { get { return objectTypes; } }

        private List<CameraGroup> mCameraGroups = new List<CameraGroup>();
        public List<CameraGroup> CameraGroups { get { return mCameraGroups; } }

        public bool HasUnitInfo(int id)
        {
            foreach (UnitActionInfo unitInfo in ObjectTypes)
            {
                if (unitInfo.ID == id)
                    return true;
            }
            return false;
        }

        public UnitActionInfo GetUnitInfo(int id)
        {
            foreach (UnitActionInfo unitInfo in ObjectTypes)
            {
                if (unitInfo.ID == id)
                    return unitInfo;
            }
            return null;
        }

        public CameraGroup GetCameraGroup(int id)
        {
            foreach (CameraGroup cameraGroup in mCameraGroups)
            {
                if (cameraGroup.ID == id)
                    return cameraGroup;
            }
            return null;
        }

        public UnitActionInfo GetUnitInfo(Object obj)
        {
            if (obj == null) return null;

            if (obj is Action)
                return GetUnitInfo(GetActionGroup(obj as Action));
            if (obj is ActionGroup)
                return GetUnitInfo(obj as ActionGroup);

            return obj as UnitActionInfo;
        }

        public UnitActionInfo GetUnitInfo(ActionGroup group)
        {
            foreach (UnitActionInfo unitInfo in ObjectTypes)
            {
                if (unitInfo.ActionGroups.Contains(group))
                    return unitInfo;
            }
            return null;
        }

        public ActionGroup GetActionGroup(Action action)
        {
            foreach (UnitActionInfo unitInfo in ObjectTypes)
            {
                foreach (ActionGroup actionGroup in unitInfo.ActionGroups)
                {
                    if (actionGroup.ActionList.Contains(action))
                        return actionGroup;
                }
            }
            return null;
        }

        public void BuildActionCache()
        {
            foreach (UnitActionInfo unitInfo in ObjectTypes)
                unitInfo.BuildActionCache();
        }
    }
}
