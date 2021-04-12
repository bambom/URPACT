using UnityEngine;
using System.Collections;

public class EquipInfo : MonoBehaviour
{
    public string[] Bones;

    // Use this for initialization
    void Start()
    {
        SkinnedMeshRenderer skinRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinRenderer != null)
        {
            Transform[] bindBones = new Transform[Bones.Length];
            Transform[] transforms = transform.parent.GetComponentsInChildren<Transform>();
            for (int i = 0; i < Bones.Length; i++)
            {
                string bone = Bones[i];
                foreach (Transform trans in transforms)
                {
                    if (trans.name == bone)
                    {
                        bindBones[i] = trans;
                        break;
                    }
                }
            }
            skinRenderer.bones = bindBones;
        }
    }
}
