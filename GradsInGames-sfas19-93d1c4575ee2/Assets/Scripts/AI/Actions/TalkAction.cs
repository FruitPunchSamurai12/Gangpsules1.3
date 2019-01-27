using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Talk")]
public class TalkAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.Talk();
    }
}
