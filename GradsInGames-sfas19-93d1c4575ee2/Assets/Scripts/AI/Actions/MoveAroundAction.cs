using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveAround")]
public class MoveAroundAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.MoveAroundLogic();
    }
}
