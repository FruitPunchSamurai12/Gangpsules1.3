using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // The total health of this unit
    
    public int m_Health = 100;
    public int m_MaxHealth = 100;
    public bool m_TakingFire = false;

    public void DoDamage(int damage)
    {
        m_Health -= damage;
        m_TakingFire = true;

        if(m_Health <= 0)
        {
            m_Health = 0;
            //Destroy(gameObject);
        }     
        if(m_Health>m_MaxHealth)
        {
            m_Health = m_MaxHealth;
        }
    }

    public bool IsAlive()
    {
        return m_Health > 0;
    }
}
