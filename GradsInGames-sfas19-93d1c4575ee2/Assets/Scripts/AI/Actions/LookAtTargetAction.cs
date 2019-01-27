using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/LookAtTarget")]
public class LookAtTargetAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.LookAtTarget();
    }
}
