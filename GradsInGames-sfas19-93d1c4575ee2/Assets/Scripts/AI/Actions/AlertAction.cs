using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Alert")]
public class AlertAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.AlertLogic();
    }
}