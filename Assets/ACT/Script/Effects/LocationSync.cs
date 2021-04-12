using UnityEngine;
using System.Collections;

public class LocationSync : MonoBehaviour {
	
	public GameObject target;
	
	Transform mCachedSelfTransform;
	Transform mCachedTargetTransform;
	Vector3 mOffset;
	
	// Use this for initialization
	void Start () {
		mCachedSelfTransform = gameObject.transform;
		mCachedTargetTransform = target.transform;
		mOffset = mCachedSelfTransform.position - mCachedTargetTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		mCachedSelfTransform.position = mCachedTargetTransform.position + mOffset;
	}
}
