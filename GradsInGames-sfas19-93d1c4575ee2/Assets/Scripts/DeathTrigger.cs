using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    // --------------------------------------------------------------

    public int m_Damage = 20;

    // --------------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if(playerController)
            {                
                Health health = playerController.GetComponent<Health>();
                if (health)
                {
                    health.DoDamage(m_Damage);
                }
                playerController.m_IsDrowning = true;
                playerController.UpdateUI();
            }
        }
    }
}
