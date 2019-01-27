using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/MultipleDecisionToAttack")]
public class MultipleDecisionToAttack : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return (!controller.IsSomethingInFrontOfMyWeapon() && !controller.IsSomethingInFrontOfMe() && controller.SeeTarget() && controller.CanAttackTarget());
    }
}