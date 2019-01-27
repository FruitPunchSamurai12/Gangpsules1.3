using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/IsDeadDecision")]
public class IsDeadDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        Health hp = controller.GetComponent<Health>();
        if (hp.m_Health<=0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
