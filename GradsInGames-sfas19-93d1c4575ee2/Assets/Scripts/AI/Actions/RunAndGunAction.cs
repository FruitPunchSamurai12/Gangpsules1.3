using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RunAndGun")]
public class RunAndGunAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.RunAndGunLogic();
    }
}
