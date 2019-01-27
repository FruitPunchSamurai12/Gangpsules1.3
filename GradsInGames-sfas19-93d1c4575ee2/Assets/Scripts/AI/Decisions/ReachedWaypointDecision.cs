using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ReachedWaypoint")]
public class ReachedWaypointDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.ReachedWaypoint();
    }
}