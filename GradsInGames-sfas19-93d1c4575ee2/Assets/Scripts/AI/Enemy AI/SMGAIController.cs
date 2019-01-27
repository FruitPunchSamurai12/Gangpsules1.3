using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGAIController : AIController
{

    public override void AttackLogic()
    {
        Vector3 whereToMove = TargetTransform().position * Mathf.PingPong(Time.time, 0.2f);  //new Vector3(TargetTransform().position.x + Mathf.PingPong(Time.time, 6), TargetTransform().position.y, TargetTransform().position.z + Mathf.PingPong(Time.time, 6));
        m_Strafe = true;
        if (m_Weapon)
        {
           // m_Weapon.GetComponent<GunLogic>().Fire();
        }
        ApplyDirection(whereToMove);
    }

    public override void RunAndGunLogic()
    {
        ApplyDirection(m_LocationToMoveTo * Mathf.PingPong(Time.time, 1f));
        m_Strafe = true;
        if (m_Weapon)
        {
            Quaternion lookRotation = Quaternion.LookRotation(TargetTransform().position - m_BulletSpawningPosition.position);
            transform.LookAt(TargetTransform().position);
            m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);
            m_Weapon.GetComponent<GunLogic>().Fire();
        }
    }
}
