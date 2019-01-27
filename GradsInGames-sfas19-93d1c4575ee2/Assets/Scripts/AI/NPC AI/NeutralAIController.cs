using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NeutralAIController : BaseAIController {


    public NPCDialogue[] m_Dialogue;
    public int m_DialogueIndex = 0;
    PlayerController m_PlayerController;
    Transform m_PlayerTransform;
    public bool m_IsTalking = false;
    public bool m_Fleeing = false;
    UIManager m_UIManager;

    public NPCDialogue GetDialogue()
    {
        NPCDialogue dialogue = m_Dialogue[m_DialogueIndex];
        m_DialogueIndex++;
        if(m_DialogueIndex>=m_Dialogue.Length)
        {
            m_DialogueIndex = 0;
        }
        return dialogue;
    }

    public void ChangeDialogue(NPCDialogue dialogue)
    {
        m_Dialogue[0] = dialogue;
    }

    public void ChangeDialogue(NPCDialogue dialogue, int index)
    {
        if (index < m_Dialogue.Length)
        {
            m_Dialogue[index] = dialogue;
        }
    }

    public override void RunAway()
    {
        m_Fleeing = true;
    }

    public override void Talk()
    {
        StopMoving();
        transform.LookAt(m_PlayerTransform.position);
        if(m_Weapon)
        {
            m_Weapon.transform.LookAt(m_PlayerTransform.position);
        }
    }

    public override void LookAtTarget()
    {
        StopMoving();    
        transform.LookAt(m_TargetToLookAt);
        if (m_Weapon)
        {
            m_Weapon.transform.LookAt(m_TargetToLookAt);
        }
    }

    public override bool AmITalking()
    {
        if(!m_UIManager.talking)
        {
            m_IsTalking = false;
            return false;
        }
        else
        {
            return m_IsTalking;
        }
    }

    public override bool ShouldIHide()
    {
        Health h = GetComponent<Health>();
        if (h.m_TakingFire)
        {
            h.m_TakingFire = false;
            return true;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius * 10f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Enemy" || hitColliders[i].tag == "Friend")
            {
                if (hitColliders[i].GetComponent<BaseAIController>().IsFiring())
                {
                    return true;
                }
            }
            else if (hitColliders[i].tag == "Player")
            {
                if (hitColliders[i].GetComponent<PlayerController>().IsGunFiring())
                {
                    return true;
                }
            }
            i++;
        }

        return false;

    }

    // Use this for initialization
    void Start () {
        m_UIManager = FindObjectOfType<UIManager>();
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        while (i < player.Length)
        {
            if (player[i].GetComponent<PlayerController>())
            {
                m_PlayerController = player[i].GetComponent<PlayerController>();
                m_PlayerTransform = player[i].transform;
                break;
            }
            i++;
        }
        if (m_TargetToLookAt == Vector3.zero)
        {
            m_TargetToLookAt = transform.position + transform.forward +transform.up*GetComponent<CharacterController>().radius/4;
        }
    }

    public void BuildNeutralAIController()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_UIManager = FindObjectOfType<UIManager>();
        tag = "Neutral";
        GetComponent<NavMeshAgent>().enabled = true;
        m_Agent = GetComponent<NavMeshAgent>();
        m_Destination = transform.position;
        instanceID = GetInstanceID();
        m_SightRange = 15.0f;
        // Get Player information
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        while (i < player.Length)
        {
            if (player[i].GetComponent<PlayerController>())
            {
                m_PlayerController = player[i].GetComponent<PlayerController>();
                m_PlayerTransform = player[i].transform;
                break;
            }
            i++;
        }
    }

    // Update is called once per frame
    void Update () {
        if(!m_PlayerController)
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            int i = 0;
            while (i < player.Length)
            {
                if (player[i].GetComponent<PlayerController>())
                {
                    m_PlayerController = player[i].GetComponent<PlayerController>();
                    m_PlayerTransform = player[i].transform;
                    break;
                }
                i++;
            }
        }
        if (!levelScriptControl)
        {
            m_State.UpdateState(this);
        }
	}
}
