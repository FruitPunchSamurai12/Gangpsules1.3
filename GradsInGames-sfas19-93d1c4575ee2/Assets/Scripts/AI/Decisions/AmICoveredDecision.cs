using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AmICovered")]
public class AmICoveredDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.AmICovered();
    }
}
