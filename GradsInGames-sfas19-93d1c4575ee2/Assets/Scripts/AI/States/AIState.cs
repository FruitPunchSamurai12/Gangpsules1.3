using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/State")]
public class AIState : ScriptableObject
{
    public float TimeLimit;
    public Action[] actions;
    public Transition[] transitions;

    public void UpdateState(BaseAIController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    private void DoActions(BaseAIController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }

    private void CheckTransitions(BaseAIController controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(controller);

            if (decisionSucceeded)
            {
                if (controller.m_State != transitions[i].trueState)
                {
                    controller.ChangeAIState(transitions[i].trueState);
                    return;
                }
            }
            else
            {
                if (controller.m_State != transitions[i].falseState)
                {
                    controller.ChangeAIState(transitions[i].falseState);
                    return;
                }
            }
        }
    }


}