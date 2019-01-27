using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoNPC : MonoBehaviour {

    Casino m_Casino;

	// Use this for initialization
	void Start () {
        m_Casino = FindObjectOfType<Casino>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        if(!m_Casino)
        {
            return;
        }
        if(m_Casino.m_Phase == levelPhase.phase1)
        {
            m_Casino.EarlyPhase2();
        }
        int random = Random.Range(0, 100);
        if(random<10)
        {
            var item = m_Casino.DropSupply();
            Vector3 itemPos = new Vector3(transform.position.x,1f,transform.position.z);
            Instantiate(item, itemPos, Quaternion.identity, m_Casino.transform);
        }
    }

}
