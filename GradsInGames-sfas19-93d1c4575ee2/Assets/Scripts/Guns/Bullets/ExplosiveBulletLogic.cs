using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBulletLogic : BulletLogic
{
    // The Explosion ParticleEmitter Prefab
    [SerializeField]
    GameObject m_ExplosionPE;

    [SerializeField]
    float m_ExplosionRadius;

    protected override void Explode()
    {
        if (m_ExplosionPE)
        {
            Instantiate(m_ExplosionPE, transform.position, transform.rotation);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!m_Active)
        {
            return;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Health health = hitColliders[i].gameObject.GetComponent<Health>();
            if (health)
            {
                float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                int dmgCalc = (int)m_ExplosionRadius - (int)distance;
                if (dmgCalc > 0)
                {
                    int percent = dmgCalc%100;
                    Debug.Log(percent);
                    health.DoDamage(m_Damage * percent/100);
                }
            }
            i++;            
        }
        Impact();
        m_Active = false;
    }
}
