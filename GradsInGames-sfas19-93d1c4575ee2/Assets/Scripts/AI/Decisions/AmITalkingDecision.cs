using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AmITalking")]
public class AmITalkingDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.AmITalking();
    }
}
