using UnityEngine;
using System.Collections;

public class DragObjectExContents : MonoBehaviour {
	public DragObjectEx dragObjectEx;
	
	void Start()
	{
		if(dragObjectEx==null)
		{
			dragObjectEx = transform.parent.GetComponent<DragObjectEx>();
		}
	}
	void OnPress (bool pressed)
	{
		if (enabled && gameObject.activeSelf && dragObjectEx != null)
		{
			dragObjectEx.OnPress(pressed);
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
        if (enabled && gameObject.activeSelf && dragObjectEx != null)
		{
			dragObjectEx.OnDrag(delta);
		}
	}
}
