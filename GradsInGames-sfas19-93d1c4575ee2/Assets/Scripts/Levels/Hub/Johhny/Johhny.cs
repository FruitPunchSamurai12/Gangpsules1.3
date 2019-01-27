using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Johhny : MonoBehaviour {

    public Hub m_Hub;

    bool changeDialogue = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NeutralAIController>().m_IsTalking)
        {
            if (changeDialogue)
            {
                m_Hub.ChangeJohhnyDialogue();
                changeDialogue = false;
            }
        }
        else
        {
            changeDialogue = true;
        }
	}
}
