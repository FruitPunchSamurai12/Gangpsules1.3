using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAIController : AIController
{
    public override bool ShouldIHide()
    {
        return true;
    }

    public override void LookAtTarget()
    {
        StopMoving();
        m_Strafe = true;
        transform.LookAt(TargetTransform().position);
        m_Weapon.transform.LookAt(TargetTransform().position);        
    }

    public override void RunAway()
    {
        Vector3 runAway = (TargetTransform().position - transform.position) * -2f;
        ApplyDirection(runAway);
    }
}
