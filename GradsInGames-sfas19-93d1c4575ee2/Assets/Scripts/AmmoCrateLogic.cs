using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrateLogic : MonoBehaviour
{
    [SerializeField]
    int m_BulletAmmo = 50;


    void OnTriggerEnter(Collider other)
    {
        GunLogic gunLogic = other.GetComponentInChildren<GunLogic>();
        if(gunLogic)
        {
            gunLogic.AddAmmo(m_BulletAmmo);
            Destroy(gameObject);
        }
    }
}
