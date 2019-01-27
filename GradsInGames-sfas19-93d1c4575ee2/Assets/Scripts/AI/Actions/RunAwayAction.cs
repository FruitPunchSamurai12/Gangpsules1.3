using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RunAway")]
public class RunAwayAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.RunAway();
    }
}