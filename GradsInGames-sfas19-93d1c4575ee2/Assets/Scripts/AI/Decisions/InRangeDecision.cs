using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/InRange")]
public class InRangeDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.TargetInRange();
    }
}
