using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CanAttack")]
public class CanAttackDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.CanAttackTarget();
    }
}