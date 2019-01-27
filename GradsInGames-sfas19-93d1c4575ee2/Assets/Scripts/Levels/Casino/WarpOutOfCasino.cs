using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpOutOfCasino : MonoBehaviour {

    public Transform warpPos;
    public Transform runLittleGirl;
    [SerializeField] Casino casino;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Neutral")
        {
            other.GetComponent<BaseAIController>().Warp(warpPos.position);
            other.GetComponent<BaseAIController>().m_LocationToMoveTo = runLittleGirl.position;
            if(casino.m_Phase == levelPhase.phase1)
            {
                casino.EarlyPhase2();
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
