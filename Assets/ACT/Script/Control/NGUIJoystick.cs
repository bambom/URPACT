using UnityEngine;
using System.Collections;

public class NGUIJoystick : MonoBehaviour
{
    static NGUIJoystick mInstance = null;
    public static NGUIJoystick instance { get { return mInstance; } }

    static bool mPressed = false;
    static public bool Pressed { get { return mPressed; } }

    static Vector2 mDelta = Vector2.zero;
    static public Vector2 Delta { get { return (Time.timeScale != 0) ? mDelta : Vector2.zero; } }

    public Transform target;
    public float direction = 100f;
    SpringPosition mSpringPos;
    Vector2 mFingerDownPos;
    int mLastFingerId = -2;

    void Awake()
    {
        ResetJoystick();
        mInstance = this;
    }

    void OnDestroy()
    {
        ResetJoystick();
        mInstance = null;
    }

    void Start()
    {
        if (target == null)
            target = transform;

        mSpringPos = target.GetComponent<SpringPosition>();
        direction = direction / UIHelper.WordToScreenModify;
    }

    void OnPress(bool pressed)
    {
        if (enabled && gameObject.activeSelf && target != null)
        {
            mPressed = pressed;
            if (pressed)
            {
                target.parent.gameObject.SetActive(true);
				target.parent.GetComponent<UIPanel>().alpha = 1f;
                Vector2 clickPos = UIHelper.ScreenPointToUIPoint(UICamera.currentTouch.pos);
                target.parent.localPosition = new Vector3(clickPos.x, clickPos.y, target.parent.localPosition.z);
                if (mSpringPos != null) mSpringPos.enabled = false;

                if (mLastFingerId == -2 || mLastFingerId != UICamera.currentTouchID)
                {
                    mLastFingerId = UICamera.currentTouchID;
                    mFingerDownPos = UIHelper.UIPointToScreenPoint(new Vector2(target.parent.localPosition.x, target.parent.localPosition.y));
                }
                OnDrag(Vector2.zero);
            }
            else
            {
                ResetJoystick();
            }
        }
    }

    public void OnDrag(Vector2 delta)
    {
        if (enabled && gameObject.activeSelf && target != null)
        {
            if (mLastFingerId == UICamera.currentTouchID)
            {
                Vector2 touchPos = UICamera.currentTouch.pos - mFingerDownPos;
				if (touchPos.sqrMagnitude > direction * direction)
				{
					touchPos.Normalize();
					touchPos *= direction;
				}
				
                float deltax = touchPos.x / (direction / 2);
                float deltay = touchPos.y / (direction / 2);
                mDelta = new Vector2(deltax, deltay) * UIHelper.UIModify;
				
                target.localPosition = new Vector3(touchPos.x, touchPos.y, target.localPosition.z) * UIHelper.WordToScreenModify;
            }
        }
    }

    public void ResetJoystick()
    {
        if (Global.GuideMode)
        {
            target.localPosition = Vector3.zero;
            target.parent.GetComponent<UIPanel>().alpha = 0.5f;
        }
        else
            target.parent.gameObject.SetActive(false);
        mDelta = Vector2.zero;
        mLastFingerId = -2;
        mPressed = false;
    }
}
