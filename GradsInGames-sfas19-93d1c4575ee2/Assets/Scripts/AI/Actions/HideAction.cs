using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Hide")]
public class HideAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.HideLogic();
    }
}
