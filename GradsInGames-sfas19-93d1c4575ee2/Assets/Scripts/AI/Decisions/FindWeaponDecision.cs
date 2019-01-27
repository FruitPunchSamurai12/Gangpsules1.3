using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/FindWeapon")]
public class FindWeaponDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return (controller.FindWeapon());
    }
}
