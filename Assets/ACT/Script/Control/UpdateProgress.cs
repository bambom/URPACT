using UnityEngine;
using System.Collections;

public class UpdateProgress : MonoBehaviour {

	// Use this for initialization
	float mOffset;
	int mCount;
	void Start () {
		mOffset = 0.0f;
	    mCount = 0;
		GetComponent<MeshRenderer>().GetComponent<Renderer>().material.SetTextureOffset("_MainTex",new Vector2(0.0f,0.0f));
	}
	
	// Update is called once per frame
	void Update () {
		mCount++;
		if(mCount == 20){
			mOffset = mOffset == 0.0f ? 1.0f : 0.0f;
			Vector2 offset = new Vector2(mOffset,0.0f);
			GetComponent<MeshRenderer>().GetComponent<Renderer>().material.SetTextureOffset("_MainTex",offset);
			mCount = 0;
		}
	}
	
	public void DestroySelf(){
	     Destroy(this);	
	}
}
