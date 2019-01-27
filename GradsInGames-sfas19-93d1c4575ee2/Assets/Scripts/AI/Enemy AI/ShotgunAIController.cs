using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAIController : AIController
{
    public override void AttackLogic()
    {
        float distance = Vector3.Distance(TargetTransform().position, transform.position);
        if(distance<m_SightRange/2f)
        {
            StopMoving();
            if (m_Weapon)
            {
                Quaternion lookRotation = Quaternion.LookRotation(TargetTransform().position - m_BulletSpawningPosition.position);
                transform.LookAt(TargetTransform().position);
                m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);
                m_LocationToMoveTo = TargetTransform().position;
                m_Weapon.GetComponent<GunLogic>().Fire();
            }
        }
        else
        {
            RunAndGunLogic();
        }
        
    }

}
