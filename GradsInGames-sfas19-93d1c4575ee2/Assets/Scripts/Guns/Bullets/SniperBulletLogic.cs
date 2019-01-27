using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBulletLogic : BulletLogic
{

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().velocity = -transform.up * m_BulletSpeed;
        GetComponent<Collider>().isTrigger = true;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!m_Active)
        {
            return;
        }
        Health health = collision.gameObject.GetComponent<Health>();
        if (health)
        {
            health.DoDamage(m_Damage);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().velocity = -transform.up * m_BulletSpeed;
        }
        else
        {
            Impact();
            m_Active = false;
        }
    }


  /*  private void OnTriggerEnter(Collider other)
    {
        if (!m_Active)
        {
            return;
        }
        Health health = other.gameObject.GetComponent<Health>();
        if (health)
        {
            health.DoDamage(m_Damage);
        }
        else
        {
            Impact();
            m_Active = false;
        }
    }*/
}
