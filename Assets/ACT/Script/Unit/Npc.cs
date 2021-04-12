using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Npc : Unit
{
    public Npc(UnitInfo unitInfo, int level)
        : base(unitInfo, level)
    {
        NpcInteract NpcScript = unitInfo.GetComponent<NpcInteract>();
        if (NpcScript == null)
        {
            GameObject goInfo = GameObject.Instantiate(Resources.Load("NpcInfoPanel")) as GameObject;
            goInfo.transform.parent = unitInfo.transform;

            float height = 1.0f;
            CapsuleCollider collider = unitInfo.GetComponent<CapsuleCollider>();
            if (collider) height = collider.height;
            else
            {
                CharacterController controller = unitInfo.GetComponent<CharacterController>();
                if (controller) height = controller.height;
            }

            goInfo.transform.localPosition = new Vector3(0.0f,
                              unitInfo.HpPointDifference.y + height,
                              0.0f);
            goInfo.transform.localScale = new Vector3(0.01f, 0.01f, 0.0f);
            NpcScript = unitInfo.gameObject.AddComponent<NpcInteract>();
        }
    }

    public override bool Hurt(Unit attacker, int damage, ECombatResult result)
    {
        return false;
    }
}
