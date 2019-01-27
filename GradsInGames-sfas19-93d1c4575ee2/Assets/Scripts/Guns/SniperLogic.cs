using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperLogic : MonoBehaviour {

    [SerializeField]
    Transform m_BulletSpawnPoint;
    LineRenderer m_Line;
    [SerializeField]
    Material line_Mat;

    GunLogic m_GunLogic;
    Equipable m_Equipable;


	// Use this for initialization
	void Start () {
        m_GunLogic = GetComponent<GunLogic>();
        m_Equipable = GetComponent<Equipable>();
        m_Line = GetComponent<LineRenderer>();
        m_Line.positionCount = 2;
        m_Line.startWidth = 0.05f;
        m_Line.endWidth = 0.05f;
        m_Line.material = line_Mat;
        m_Line.startColor = Color.red;
        m_Line.endColor = Color.red;
        m_Line.enabled = false;

    }

    private void FixedUpdate()
    {
       if(m_Equipable.AmIEquiped())
        {
            if (m_GunLogic.CanIShoot())
            {
                m_Line.enabled = true;
                RaycastHit hit;
                if (Physics.Raycast(m_BulletSpawnPoint.position, m_BulletSpawnPoint.forward, out hit))
                {
                    m_Line.SetPosition(0, m_BulletSpawnPoint.position);
                    m_Line.SetPosition(1, hit.point + hit.normal);
                }
            }
            else
            {
                m_Line.enabled = false;
            }
        }
        else
        {
            m_Line.enabled = false;
        }
    }

}
