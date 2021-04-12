using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointByPoint : MonoBehaviour {
	public float oneTime =0.4f;
	public List<UISprite> sprites = new List<UISprite>();
	// Use this for initialization
	void Start () {
		if(sprites.Count <= 0)
		{
			UISprite[] arrays = transform.GetComponentsInChildren<UISprite>();
			foreach (UISprite item in arrays) {
				sprites.Add(item);
			}
			sprites.Sort((x, y) => x.name.CompareTo(y.name));
		}
	}

	void InitAllSprite ()
	{
		foreach (UISprite item in sprites) {
//			item.transform.localScale = Vector3.zero;
//			item.color = new Color(1,1,1,0.1f);
			item.gameObject.SetActive(false);
		}
	}
	
	float delayTime = 0;
	int index = 0;
	// Update is called once per frame
	void Update () {
		if(delayTime <= oneTime)
		{
			delayTime += Time.deltaTime;
		}
		else
		{
			delayTime = 0;
			if(index >= sprites.Count)
			{
				index = 0;
				InitAllSprite ();
			}
			else
			{
				sprites[index].gameObject.SetActive(true);
//				iTween.ScaleFrom(sprites[index].gameObject,sprites[index].transform.localScale*1.5f,0.3f);
//				sprites[index].color = new Color(1,1,1,1);
				index++;
			}
		}
	}
}
