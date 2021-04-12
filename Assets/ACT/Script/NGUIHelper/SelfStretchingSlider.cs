using UnityEngine;
using System.Collections;

public class SelfStretchingSlider : MonoBehaviour {
	public UISlider mSlider;
	public Transform mBackground;
	public Transform mForeground;
	
	void Awake()
	{
		if(mSlider!=null)
		{
			mSlider.fullSize = new Vector2(mSlider.fullSize.x + (mSlider.fullSize.x* (UIHelper.UIModify -1)) / 2,mSlider.fullSize.y);
			
			mBackground.localScale = new Vector3(mBackground.localScale.x + (mBackground.localScale.x * (UIHelper.UIModify - 1)) / 2,mBackground.localScale.y,mBackground.localScale.z);
		}
	}
}
