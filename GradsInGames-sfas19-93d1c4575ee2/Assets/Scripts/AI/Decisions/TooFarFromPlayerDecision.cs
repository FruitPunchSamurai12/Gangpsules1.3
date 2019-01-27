using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/TooFarFromPlayer")]
public class TooFarFromPlayerDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.TooFarFromPlayer();
    }
}

