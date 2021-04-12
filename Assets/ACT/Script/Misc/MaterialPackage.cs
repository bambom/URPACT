using UnityEngine;
using System.Collections;

public class MaterialPackage : MonoBehaviour {
	
	public Material[] materials;
	
	// Use this for initialization
	void Start () {
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void ChangeMaterial(string idx) {
		int index = 0;
		if (!int.TryParse(idx, out index))
			return;
		
		if (index >= materials.Length)
			return;
		
		Material mat = materials[index];
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (!meshRenderer)
			return;
		
		meshRenderer.material = mat;
	}
}
