using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StrengthWnd : Window<StrengthWnd>
{
	public override string PrefabName { get { return "StrengthWnd"; } }
	UILabel mStrengthLabel;
	UILabel mGemCountLabel;
	UISlider mStrengthSlider;
	const float mPhysicMax = 12.0f;
	
    protected override bool OnOpen()
    {	
		mStrengthLabel = Control("StrengthLabel").GetComponent<UILabel>();
		mGemCountLabel = Control("GemCountLabel").GetComponent<UILabel>();
		mStrengthSlider= Control("StrengthSlider").GetComponent<UISlider>();
		
		UIEventListener.Get(Control("BackButton")).onClick = OnClickBtnClose;
		UIEventListener.Get(Control("BuyButton")).onClick = OnClickBuyStrength;
	    Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	private void OnClickBtnClose(GameObject go)
	{
		Close();
	}
	
	private void OnClickBuyStrength(GameObject go)
	{
		if(float.Parse(mStrengthLabel.text) >= mPhysicMax){
			MessageBoxWnd.Instance.Show("魂力全满!",MessageBoxWnd.StyleExt.Alpha);
			return;
		}
	    MessageBoxWnd.Instance.Show("确认购买?",MessageBoxWnd.Style.OK_CANCLE);
		MessageBoxWnd.Instance.OnClickedOk = OnClickConfirmBuy;
		MessageBoxWnd.Instance.OnClickedCancle = OnClickCancelBuy;
	}
	
	private void Init()
	{
		StrengthInfo phy = PlayerDataManager.Instance.StrengthInfo;
		StrengthBase sBase =  StrengthBaseManager.Instance.GetItem(phy.time+1);
		mGemCountLabel.text = sBase.Gem.ToString();
		UpdateDataInfo(phy.value);
	}
	
	private void OnClickConfirmBuy(GameObject go)
	{
		Request(new BuyStrengthCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}
			RequestPlayerInfo();
		});
	}
	
	private void RequestPlayerInfo()		
	{
		RequestAttribInfo();
		RequestPhysicInfo();
	}
	
	private void RequestAttribInfo()
	{
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.OnClickedCancle = OnClickedCancle;
				MessageBoxWnd.Instance.OnClickedOk = OnClickedOk;
				MessageBoxWnd.Instance.Show(err);
				if(!Exist)
					Instance.Open();
                return;
            }

            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
			{
				PlayerDataManager.Instance.OnMainAttrib(attrib);
			}
        });
	}
	
	private	void RequestPhysicInfo()
	{
		Request(new GetStrengthCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}
            StrengthInfo Physic = (response != null) ? response.Parse<StrengthInfo>() : null;
            if (Physic != null)
            {
                PlayerDataManager.Instance.OnStrengthInfo(Physic);
            }
			OnGetPhysicSuccess();
		});
	}
	
	private void OnGetPhysicSuccess()
	{
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		StrengthInfo phy = PlayerDataManager.Instance.StrengthInfo;
		InGameMainWnd.Instance.ResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,phy.value);
		UpdateDataInfo(phy.value);
	}
	
	private void UpdateDataInfo(int phyValue)
	{
		float curPhyValue = phyValue;
		mStrengthLabel.text = curPhyValue.ToString();
		mStrengthSlider.sliderValue = curPhyValue / mPhysicMax;
	}
	
	void OnClickedOk( GameObject go )
	{
		if(!Exist)
			Instance.Open();
	}
	
	void OnClickedCancle( GameObject go )
	{
		if(!Exist)
			Instance.Open();
	}
	
	private void OnClickCancelBuy(GameObject go)
	{
		
	}
}
