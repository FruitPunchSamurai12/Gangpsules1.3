using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/FindHidingPlace")]
public class FindHidingPlaceDecision : Decision
{
    public override bool Decide(BaseAIController controller)
{
    return controller.FindPlaceToHide();
}
}
