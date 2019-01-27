using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSeeThroughWalls : MonoBehaviour {

    PlayerController m_PlayerController;
    Camera m_Camera;
    MeshRenderer m_MeshRenderer;


    // Use this for initialization
    void Start () {
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
        m_Camera = FindObjectOfType<Camera>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if(!m_Camera)
        {
            m_Camera = FindObjectOfType<Camera>();
        }
        if(!m_PlayerController)
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
        if (m_Camera && m_PlayerController)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_Camera.transform.position, (m_PlayerController.transform.position - m_Camera.transform.position).normalized, out hit))
            {
                Debug.DrawRay(m_Camera.transform.position, m_PlayerController.transform.position - m_Camera.transform.position);
                if (hit.collider.gameObject == gameObject)
                {
                    m_MeshRenderer.enabled = false;
                }
                else
                {
                    m_MeshRenderer.enabled = true;
                }
            }
        }
    }
}
