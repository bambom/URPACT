using UnityEngine;
using System.Collections;

public class InfectionEffect : MonoBehaviour {
	public AudioClip audioClip;
	public GameObject eventReceiver;
	public string funtion;
	
	float maxdelay = 0f;
	RaycastHit hit;
	Transform lastT;
	
	// Update is called once per frame
	public void InfectionUpdate (Transform targetPanel) 
	{
		Vector3 fwd = transform.position + transform.forward * 100;
        if (Physics.Raycast(transform.position, fwd, out hit))
		{
			if(hit.transform.parent.parent != targetPanel) return;
			float delay =  Mathf.Abs(transform.position.y - hit.transform.position.y);
			
			if(maxdelay == 0)
			{
				maxdelay = delay;
				NGUITools.PlaySound(audioClip);
			}
			if(hit.transform!=lastT)
			{
				lastT = hit.transform;
				eventReceiver.SendMessage(funtion, lastT.gameObject);
			}
			lastT.localScale = new Vector3(1 + maxdelay - delay - 0.2f,1 + maxdelay - delay - 0.2f,1);
			lastT.Find("Icon").GetComponent<UISprite>().alpha = 1 - delay * 3;
		}
		else
		{
			maxdelay = 0f;
		}
	}
	
	public void RestrictWithinSelectedItem(Transform targetPanel,Vector3 InitialLocalPosition)
	{
		Transform parent = lastT.parent;
		UIGrid grid = parent.GetComponent<UIGrid>();
		
		int max = parent.childCount;
		int num = -1;
		for (int i = 0; i < max; i++) {
			if(lastT == parent.GetChild(i))
			{
				num = i - 1;
			}
		}
		Vector3 targetposition = new Vector3(InitialLocalPosition.x,InitialLocalPosition.y + grid.cellHeight * num,InitialLocalPosition.z);
		SpringPanel.Begin(targetPanel.gameObject, targetposition ,13);
	}
	
	
}
