using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CanHear")]
public class CanHearDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.HearTarget();
    }
}