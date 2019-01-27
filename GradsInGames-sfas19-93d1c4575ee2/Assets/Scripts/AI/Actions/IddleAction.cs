using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Iddle")]
public class IddleAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.IddleLogic();
    }
}