using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callum : MonoBehaviour {

    Casino m_Casino;
    PlayerController m_PlayerController;
    NeutralAIController m_Callum;
    [SerializeField] AIState grabABottleCallum;
    [SerializeField] AIState callumTalkState;
    [SerializeField] AIState callumIddleState;
    [SerializeField] Waypoint callumWaypoint;
    [SerializeField] Throwable bottle;
    [SerializeField] Transform dropBottlePosition;
    [SerializeField] GameObject m_Camera;

    [SerializeField] NPCDialogue afterServeDialogue;
    [SerializeField] NPCDialogue[] phase2Dialogues;
    [SerializeField] NPCDialogue phase6Dialogue;


    bool hasServed = false;
    bool isServing = false;

    // Use this for initialization
    void Start () {
        m_Casino = FindObjectOfType<Casino>();
        m_Callum = GetComponent<NeutralAIController>();
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        while (i < player.Length)
        {
            if (player[i].GetComponent<PlayerController>())
            {
                m_PlayerController = player[i].GetComponent<PlayerController>();
                break;
            }
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_PlayerController)
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            int i = 0;
            while (i < player.Length)
            {
                if (player[i].GetComponent<PlayerController>())
                {
                    m_PlayerController = player[i].GetComponent<PlayerController>();
                    break;
                }
                i++;
            }
        }
        if (m_Casino.m_Phase == levelPhase.phase1)
        {
            if (m_Callum.AmITalking() && m_Callum.m_DialogueIndex == 0)
            {
                if (!isServing)
                {
                    m_Camera.GetComponent<CameraFollow>().CloseIn(transform);
                    m_PlayerController.m_DialogueHasControl = true;
                    m_Callum.ChangeAIState(grabABottleCallum);
                    isServing = true;
                }
                else if (bottle.m_State == throwableStates.iddle && !hasServed)
                {
                    //if (Vector3.Distance(m_Callum.m_CurrentWaypoint.transform.position, m_Callum.transform.position) < 2f)
                    if(m_Callum.m_State == callumIddleState)
                    {
                        bottle.PickUp(m_Callum);
                    }
                }
                else if (!hasServed)
                {
                    if (m_Callum.ReachedWaypoint() && Vector3.Distance(transform.position, dropBottlePosition.position) < 3)
                    {
                        bottle.Drop(dropBottlePosition.position);
                        hasServed = true;
                        m_PlayerController.m_DialogueHasControl = false;
                        m_Callum.ChangeAIState(callumTalkState);
                        m_Callum.ChangeDialogue(afterServeDialogue, 2);
                        m_Camera.GetComponent<CameraFollow>().m_CloseIn = false;
                    }
                }
            }
        }
        else if (m_Casino.m_Phase == levelPhase.phase2)
        {
            m_Callum.ChangeDialogue(phase2Dialogues[0], 0);
            m_Callum.ChangeDialogue(phase2Dialogues[1], 1);
            m_Callum.ChangeDialogue(phase2Dialogues[2], 2);
        }
        else if (m_Casino.m_Phase == levelPhase.phase6)
        {
            m_Callum.ChangeDialogue(phase6Dialogue, 0);
            m_Callum.ChangeDialogue(phase6Dialogue, 1);
            m_Callum.ChangeDialogue(phase6Dialogue, 2);
            if (m_Callum.m_IsTalking)
            {
                m_Casino.m_LevelWon = true;
            }
        }
    }
}
