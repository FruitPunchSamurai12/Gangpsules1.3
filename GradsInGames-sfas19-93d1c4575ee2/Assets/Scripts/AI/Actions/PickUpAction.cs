using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/PickUp")]
public class PickUpAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.PickUpWeapon();
    }
}
