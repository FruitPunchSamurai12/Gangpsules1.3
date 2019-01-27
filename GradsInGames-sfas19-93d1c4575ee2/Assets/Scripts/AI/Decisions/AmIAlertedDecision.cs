using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AmIAlerted")]
public class AmIAlertedDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.AmIAlerted();
    }
}