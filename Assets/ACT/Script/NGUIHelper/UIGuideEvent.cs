using UnityEngine;

public class UIGuideEvent : MonoBehaviour
{
	public delegate void VoidDelegate (GameObject go);
	public delegate void BoolDelegate (GameObject go, bool state);

	public VoidDelegate onClick;
	public BoolDelegate onPress;
	
	void OnClick ()					{ if (onClick != null) onClick(gameObject); }
	void OnPress (bool isPressed)	
	{ 
		if (onPress != null) 
		{
			onPress(gameObject, isPressed); 
		}
	}
	/// <summary>
	/// Get or add an event listener to the specified game object.
	/// </summary>

	static public UIGuideEvent Get (GameObject go)
	{
		UIGuideEvent listener = go.GetComponent<UIGuideEvent>();
		if (listener == null) listener = go.AddComponent<UIGuideEvent>();
		return listener;
	}
}