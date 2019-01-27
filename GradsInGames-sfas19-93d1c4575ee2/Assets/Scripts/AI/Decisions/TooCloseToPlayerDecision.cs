using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/TooCloseToPlayer")]
public class TooCloseToPlayerDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.TooCloseToPlayer();
    }
}
