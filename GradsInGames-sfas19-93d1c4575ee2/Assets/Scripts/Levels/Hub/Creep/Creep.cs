using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : NeutralAIController {

    [SerializeField]
    Transform suzan;

    public override void LookAtTarget()
    {
        StopMoving();
        transform.LookAt(suzan.position);
    }
}
