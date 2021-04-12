using UnityEngine;
using System.Collections;

public class CharacterControllerPush : MonoBehaviour {
	public enum PushDirection { Local, World };
	public Vector3 force;
	public PushDirection direction;

	
	void OnCollisionStay(Collision collision) 
	  {
        UnitInfo unitInfo = collision.gameObject.GetComponent<UnitInfo>();
        if (unitInfo != null)
		  {
			if (direction == PushDirection.Local)
            	unitInfo.Unit.Move(transform.rotation * force * Time.deltaTime);
			else
				unitInfo.Unit.Move(force * Time.deltaTime);
		  }
	  }
}
