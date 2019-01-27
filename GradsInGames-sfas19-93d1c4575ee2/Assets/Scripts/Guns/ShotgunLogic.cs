using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunLogic : GunLogic {

    public override void Fire()
    {
        if (m_CanShoot)
        {
            m_CanShoot = false;
            if (m_BulletPrefab)
            {
                // Reduce the Ammo count
                m_BulletAmmo -= 5;

                // Create the Projectile from the Bullet Prefab
                var rotation = m_BulletSpawnPoint.rotation;
                var rotation2 = rotation * Quaternion.Euler(0,5, 0);
                var rotation3 = rotation * Quaternion.Euler(0, -5, 0);
                var rotation4 = rotation * Quaternion.Euler(0, 10, 0);
                var rotation5 = rotation * Quaternion.Euler(0, -10, 0);

                var bullet1 = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, rotation * m_BulletPrefab.transform.rotation);
                var bullet2 = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, rotation2 * m_BulletPrefab.transform.rotation);
                var bullet3 = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, rotation3 * m_BulletPrefab.transform.rotation);
                var bullet4 = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, rotation4 * m_BulletPrefab.transform.rotation);
                var bullet5 = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, rotation5 * m_BulletPrefab.transform.rotation);
                bullet1.GetComponent<BulletLogic>().m_BulletOwnerID = m_GunOwnerID;
                bullet2.GetComponent<BulletLogic>().m_BulletOwnerID = m_GunOwnerID;
                bullet3.GetComponent<BulletLogic>().m_BulletOwnerID = m_GunOwnerID;
                bullet4.GetComponent<BulletLogic>().m_BulletOwnerID = m_GunOwnerID;
                bullet5.GetComponent<BulletLogic>().m_BulletOwnerID = m_GunOwnerID;


                // Play Particle Effects
                PlayGunVFX();

                // Play Sound effect
                if (m_AudioSource && m_BulletShot)
                {
                    m_AudioSource.PlayOneShot(m_BulletShot);
                }
            }
        }
    }
}
