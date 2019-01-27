using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ReachedDestination")]
public class ReachedDestinationDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.ReachedDestination();
    }
}
