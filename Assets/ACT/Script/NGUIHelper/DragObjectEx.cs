//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Allows dragging of the specified target object by mouse or touch, optionally limiting it to be within the UIPanel's clipped rectangle.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class DragObjectEx : IgnoreTimeScale
{
	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring,
	}

	/// <summary>
	/// Target object that will be dragged.
	/// </summary>

	public Transform target;

	/// <summary>
	/// Scale value applied to the drag delta. Set X or Y to 0 to disallow dragging in that direction.
	/// </summary>

	public Vector3 scale = Vector3.one;
	
	public Vector3 beginPos;
	public Vector3 endPos;
	
	public Vector4 min;
	public Vector4 max;
	
	Plane mPlane;
	Vector3 mLastPos;
	UIPanel mPanel;
	bool mPressed = false;

    public bool Pressed { get { return mPressed; } }
	
	/// <summary>
	/// Find the panel responsible for this object.
	/// </summary>
	
	void Start()
	{
		transform.localPosition = beginPos;
	}
	
	void FindPanel ()
	{
		mPanel = (target != null) ? UIPanel.Find(target.transform, false) : null;
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	public void OnPress (bool pressed)
	{
		if (enabled && gameObject.activeSelf && target != null)
		{
			mPressed = pressed;

			if (pressed)
			{
				// Disable the spring movement
				SpringPosition sp = target.GetComponent<SpringPosition>();
				if (sp != null) sp.enabled = false;

				// Remember the hit position
				mLastPos = UICamera.lastHit.point;

				// Create the plane to drag along
				Transform trans = UICamera.currentCamera.transform;
				mPlane = new Plane((mPanel != null ? mPanel.cachedTransform.rotation : trans.rotation) * Vector3.back, mLastPos);
			}
			else
			{
				if(target.localPosition.x > min.x)
					SpringPosition.Begin(gameObject,beginPos,10);
				else if(target.localPosition.x < max.x)
					SpringPosition.Begin(gameObject,endPos,10);
			}
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	public void OnDrag (Vector2 delta)
	{
		if (enabled && gameObject.activeSelf && target != null)
		{
			if(target.localPosition.x > min.x || target.localPosition.x < max.x) return;
			
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float dist = 0f;

			if (mPlane.Raycast(ray, out dist))
			{
				Vector3 currentPos = ray.GetPoint(dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;

				if (offset.x != 0f || offset.y != 0f)
				{
					offset = target.InverseTransformDirection(offset);
					offset.Scale(scale);
					offset = target.TransformDirection(offset);
				}

				// Adjust the position
				target.localPosition += offset;
			}
		}
	}

}