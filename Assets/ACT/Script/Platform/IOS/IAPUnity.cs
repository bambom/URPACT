using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IAPUnity
{
	[DllImport ("__Internal")]
	static extern bool _CanPurchase();
	
	[DllImport ("__Internal")]
	public static extern void _Purchase(string productId);
	
	public static void BuyGem(string productId)
	{
		Debug.Log("IAPUnity BuyGem");
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Debug.Log("IAPUnit _BuyGem");
			_Purchase(productId);
		}
	}
}




