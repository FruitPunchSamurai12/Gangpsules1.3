using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ShouldIHide")]
public class ShouldIHideDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.ShouldIHide();
    }
}
