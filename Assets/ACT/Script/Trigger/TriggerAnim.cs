using UnityEngine;
using System.Collections;

public class TriggerAnim : MonoBehaviour
{
    public Transform Target;
    public AnimationClip TriggerEnterAnim;
    public AnimationClip TriggerLeaveAnim;

    void Start()
    {
        if (!Target)
            Target = transform;
    }

    void OnTriggerEnter()
    {
        Target.GetComponent<Animation>().Play(TriggerEnterAnim.name);
    }

    void OnTriggerExit()
    {
        Target.GetComponent<Animation>().Play(TriggerLeaveAnim.name);
    }
}
