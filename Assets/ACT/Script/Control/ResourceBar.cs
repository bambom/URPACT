using UnityEngine;
using System.Collections;

public class ResourceBar
{
	public enum ResType
	{
		RT_NonPhysic,
		RT_All,
	}
	
	
	public string PrefabName { get { return "ResourceBar"; } }
	
	GameObject mResBar;
	
	UILabel mGoldLabel;
    UILabel mGemLabel;
	UILabel mSPLabel;
	UILabel mPhysicLabel;
	
	float mPhysicMax = 12.0f;
	GameObject mAddMoney;
	GameObject mAddPhysic;
	
	GameObject mPhysicRoot;
	UISlider   mPhysicSlider;
	ResType mRType = ResType.RT_All;
	
	public delegate void ClickAddMoney(GameObject go);
	public ClickAddMoney ClickedAddMoney;
	public delegate void ClickAddPhysic(GameObject go);
	public ClickAddPhysic ClickedAddPhysic;	
	
	public ResourceBar(GameObject goParent)
	{
		if(mResBar == null){
			mResBar = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
			
			mResBar.transform.parent = goParent.transform;
			mResBar.transform.localPosition = Vector3.zero;
			mResBar.transform.localScale = Vector3.one;
		}
		
		mPhysicRoot = Control("PhysicRoot");
        mGoldLabel = Control("LabelGold").GetComponentInChildren<UILabel>();
        mGemLabel = Control("LabelGem").GetComponentInChildren<UILabel>();	
		mSPLabel = Control("LabelSP").GetComponent<UILabel>();
		mPhysicLabel = Control("LabelPhysic").GetComponentInChildren<UILabel>();
		mPhysicSlider = Control("Slider").GetComponentInChildren<UISlider>();
		
		mAddPhysic = Control("AddPhysic");
		
		UIEventListener.Get(Control("AddGold")).onClick = OnClickAddMoney;
		UIEventListener.Get(Control("AddGem")).onClick = OnClickAddMoney;
		UIEventListener.Get(mAddPhysic).onClick = OnClickAddPhysic;
	}
	
	private void OnClickAddPhysic(GameObject go)
	{
		//if(mPhysicMax)
		if( null != ClickedAddPhysic)
		{
			ClickedAddPhysic(go);
			int CurPhy = int.Parse(mPhysicLabel.text);
			if( CurPhy >= mPhysicMax )
			{
				MessageBoxWnd.Instance.Show("StrengthIsFull",MessageBoxWnd.StyleExt.Alpha);
				return;
			}	
			
			if(!StrengthWnd.Exist)
				StrengthWnd.Instance.Open();
		    StrengthWnd.Instance.WndObject.transform.localPosition = new Vector3(0,0,-20f);
		}
	}
	
	private void OnClickAddMoney(GameObject go)
	{
		if(null != ClickedAddMoney)
		{
			ClickedAddMoney(go);
			if( !RechargeWnd.Exist )
				RechargeWnd.Instance.Open();
		}
	}
	
	private GameObject Control(string name)
    {
        return Control(name, mResBar);
    }
	
	private GameObject Control(string name, GameObject parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children){
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }
	
	private string MoneyFormat(int money)
	{
		if(money < 0)
			money = 0;
		return string.Format("{0:C0}",money).Substring(1);
	}

    public void Update(MainAttrib attrib)
    {
        Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
    }

    public void Update(int gold, int gem, int sp,int physic)
    {
        if (mResBar == null)
            return;

        mGoldLabel.text = MoneyFormat(gold);
        mGemLabel.text = MoneyFormat(gem);
		mSPLabel.text = sp.ToString();
        if (physic != -1)
        {
            float Ph = physic;
            mPhysicSlider.sliderValue = Ph / mPhysicMax;
            mPhysicLabel.text = physic.ToString();
        }
    }

	
	public void UpdateByResType(ResType Rt)
	{
		if(Rt == ResType.RT_NonPhysic){
			mPhysicRoot.SetActive(false);
		}else{
			if( !mPhysicRoot.activeSelf ) mPhysicRoot.SetActive(true);
		}
	}
}

