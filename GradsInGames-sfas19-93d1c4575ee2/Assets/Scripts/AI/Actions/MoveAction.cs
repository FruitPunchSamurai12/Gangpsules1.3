using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/Move")]
public class MoveAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.MovementLogic();
    }
}
