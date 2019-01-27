using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Follow")]
public class FollowAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.FollowPlayer();
    }
}