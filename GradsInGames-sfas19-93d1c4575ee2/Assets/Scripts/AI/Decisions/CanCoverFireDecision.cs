using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CanCoverFire")]
public class CanCoverFireDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.CanCoverFire();
    }
}