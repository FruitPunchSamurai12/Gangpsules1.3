﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Die")]
public class DeathAction : Action
{
    public override void Act(BaseAIController controller)
    {
        controller.Die();
    }
}