using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CanSee")]
public class CanSeeDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.SeeTarget();
    }
}