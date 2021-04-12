using UnityEngine;
using System.Collections.Generic;

public enum EPropsAttrib
{
    Slot = 0,
}

// PropsItem
//  it is a shadow copy of item
//  just for use item as a props easy.
public class PropsItem
{
    Item mItem;
    PropsBase mPropsBase;

    public Item Item { get { return mItem; } }
    public PropsBase PropsBase { get { return mPropsBase; } }
    public int Slot { get { return GetAttrib(EPropsAttrib.Slot); } }

    public int GetAttrib(EPropsAttrib attrib) { return mItem.GetAttrib(attrib.ToString()); }

    public PropsItem(Item item)
    {
        mItem = item;

        mPropsBase = PropsBaseManager.Instance.GetItem(item.Base);
        if (mPropsBase == null)
            Debug.LogError("PropBase not found in table: " + item.Base);
    }
}
