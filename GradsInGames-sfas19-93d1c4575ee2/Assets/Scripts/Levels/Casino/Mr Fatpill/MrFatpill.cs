using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrFatpill : ShotgunAIController {

    public override bool AmIAlerted()
    {
        AIController[] goons = FindObjectsOfType<AIController>();
        int i = 0;
        while(i<goons.Length)
        {
            if(goons[i].gameObject.name == gameObject.name)
            {
                i++;
                continue;
            }
            if(goons[i].engagingTarget)
            {
                m_LocationToMoveTo = TargetTransform().position;
                return true;
            }
            i++;
        }
        return false;
    }
}
