using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/LookAround")]
public class LookAroundAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.LookAroundLogic();
    }
}