using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/StayedTooLong")]
public class StayedTooLongDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.CheckIfCountDownElapsed(controller.m_State.TimeLimit);
    }
}

