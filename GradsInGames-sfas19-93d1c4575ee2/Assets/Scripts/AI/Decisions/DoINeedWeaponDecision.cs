using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/DoINeedWeapon")]
public class DoINeedWeaponDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return (controller.DoINeedWeapon());
    }
}
