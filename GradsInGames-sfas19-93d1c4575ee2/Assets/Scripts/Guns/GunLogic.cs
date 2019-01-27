using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum gunTypes
{
    pistol,
    smg,
    ak47,
    shotgun,
    sniper
}

public class GunLogic : MonoBehaviour
{
    public int m_GunOwnerID;

    public gunTypes m_GunType;

    // The Bullet Prefab
    [SerializeField]
    protected GameObject m_BulletPrefab;

    // The Explosive Bullet Prefab
    [SerializeField]
    GameObject m_ExplosiveBulletPrefab;

    // The Bullet Spawn Point
    public Transform m_BulletSpawnPoint;

    [SerializeField]
    protected float m_ShotCooldown = 0f;

    [SerializeField]
    protected float m_FireRate = 0.3f;

    protected bool m_CanShoot = true;

    // VFX
    [SerializeField]
    protected ParticleSystem m_Flare;

    [SerializeField]
    protected ParticleSystem m_Smoke;

    [SerializeField]
    protected ParticleSystem m_Sparks;

    // SFX
    [SerializeField]
    protected AudioClip m_BulletShot;

    [SerializeField]
    protected AudioClip m_GrenadeLaunched;

    // The AudioSource to play Sounds for this object
    protected AudioSource m_AudioSource;

    public float m_Range = 20f;
    public int m_BulletAmmo;
    public int m_MinBullets;
    public int m_MaxBullets;


    private void Awake()
    {
        ResetGun();
    }

    // Use this for initialization
    void Start ()
    {
        m_AudioSource = GetComponent<AudioSource>();      
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_CanShoot)
        {
            m_ShotCooldown += Time.deltaTime;
            if (m_ShotCooldown > m_FireRate)
            {
                m_ShotCooldown = 0;
                m_CanShoot = true;
            }
        }
    }

    public void ResetGun()
    {
        m_BulletAmmo = Random.Range(m_MinBullets, m_MaxBullets);
    }

    public bool CanIShoot()
    {
        return m_CanShoot;
    }

    public void Shooting()
    {
        Fire();
    }


    virtual public void Fire()
    {
        if (m_CanShoot)
        {
            m_CanShoot = false;
            if (m_BulletPrefab)
            {
                // Reduce the Ammo count
                --m_BulletAmmo;

                // Create the Projectile from the Bullet Prefab
                
                var bullet = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, m_BulletSpawnPoint.rotation * m_BulletPrefab.transform.rotation);
                bullet.GetComponent<BulletLogic>().m_BulletOwnerID = m_GunOwnerID;

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

    public bool IsGunFiring()
    {
        return m_AudioSource.isPlaying;
    }

   

    protected void PlayGunVFX()
    {
        if (m_Flare)
        {
            m_Flare.Play();
        }

        if (m_Sparks)
        {
            m_Sparks.Play();
        }

        if (m_Smoke)
        {
            m_Smoke.Play();
        }
    }

    public void AddAmmo(int bullets)
    {
        m_BulletAmmo += bullets;
        gameObject.GetComponent<PlayerController>().UpdateUI();
    }
}
