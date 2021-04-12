using UnityEngine;
using System.Collections;

public class Guidance : MonoBehaviour
{
    float tempLeft = 0;
    float tempTop = 0;
	float tempRight = 0;
    float tempDown = 0;

    void Awake()
    {
        tempLeft = 0 +transform.localScale.x / 2;
		tempRight = Screen.width - transform.localScale.x / 2;
		
        tempTop = Screen.height  - transform.localScale.x / 2;
		tempDown = 0 + transform.localScale.x / 2;
    }

    public void Show(Vector3 p)
    {
        float x = Mathf.Clamp(p.x, tempLeft, tempRight);
        float y = Mathf.Clamp(p.y, tempDown, tempTop);
		
		if (x == tempLeft)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (x == tempRight)
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        else if (y == tempTop)
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        else if (y == tempDown)
            transform.localRotation = Quaternion.Euler(0, 0, 90);
		
        transform.localPosition = UIHelper.ScreenPointToUIPoint(new Vector3(x, y, transform.localPosition.z));
        
    }
}
