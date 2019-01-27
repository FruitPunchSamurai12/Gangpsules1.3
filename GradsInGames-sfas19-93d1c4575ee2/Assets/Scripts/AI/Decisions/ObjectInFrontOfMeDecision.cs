using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/SomethingInFrontOfMe")]
public class ObjectInFrontOfMeDecision : Decision
{
    public override bool Decide(BaseAIController controller)
    {
        return controller.IsSomethingInFrontOfMe();
    }
}