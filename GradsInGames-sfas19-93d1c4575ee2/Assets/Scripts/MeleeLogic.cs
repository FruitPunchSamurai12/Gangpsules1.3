using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeLogic : MonoBehaviour {

    public int m_Damage = 50;
    public int m_WeaponOwnerID = 0;
    public GameObject m_Dad;
    public Vector3 m_StartPosition;
    public Vector3 m_EndPosition;
    float distance;

    public float m_Time = 0f;
    public float m_TimeLimit = 0.5f;

    // Use this for initialization
    void Start () {
        m_StartPosition = m_Dad.transform.position + m_Dad.transform.right * 0.75f + m_Dad.transform.forward ;
        m_EndPosition = m_Dad.transform.position - m_Dad.transform.right * 0.75f + m_Dad.transform.forward ;
        transform.position = m_StartPosition;
        distance = Vector3.Distance(m_StartPosition, m_EndPosition);
    }
	
	// Update is called once per frame
	void Update () {
        m_Time += Time.deltaTime;
        m_StartPosition = m_Dad.transform.position + m_Dad.transform.right * 0.75f + m_Dad.transform.forward;
        m_EndPosition = m_Dad.transform.position - m_Dad.transform.right * 0.75f + m_Dad.transform.forward ;
        MeleeAttack();
	}

    void MeleeAttack()
    {
        transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, m_Time / m_TimeLimit);
        Vector3 middlePoint = (m_EndPosition+m_StartPosition) / 2;
        Vector3 lookAt = middlePoint + m_Dad.transform.forward * distance;
        Quaternion lookRotation = Quaternion.LookRotation(lookAt - transform.position);
        transform.rotation = lookRotation * Quaternion.AngleAxis(-90, Vector3.left);
        if (m_Time>m_TimeLimit)
        {
            m_Time = 0;
            transform.position = m_StartPosition;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var bot = other.GetComponent<BaseAIController>();
        if (bot)
        {
            if (bot.instanceID != m_WeaponOwnerID)
            {
                Health health = other.GetComponent<Health>();
                if (health)
                {
                    health.DoDamage(m_Damage);
                }
            }
        }
    }
}
