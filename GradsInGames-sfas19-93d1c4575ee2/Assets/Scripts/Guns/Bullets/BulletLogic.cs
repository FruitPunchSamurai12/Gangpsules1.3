using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public int m_BulletOwnerID;

    // The lifetime of the bullet
    [SerializeField]
    protected float m_BulletLifeTime = 2.0f;

    // The speed of the bullet
    [SerializeField]
    protected float m_BulletSpeed = 15.0f;

    // The damage of the bullet
    [SerializeField]
    protected int m_Damage = 20;

    protected bool m_Active = true;

    // Use this for initialization
    void Start()
    {
        // Add velocity to the bullet
        GetComponent<Rigidbody>().velocity = -transform.up * m_BulletSpeed;
        Physics.IgnoreLayerCollision(2, 2, true);
    }

    // Update is called once per frame
    void Update ()
    {
        m_BulletLifeTime -= Time.deltaTime;
        if(m_BulletLifeTime < 0.0f)
        {
            Impact();
        }    
    }

    virtual protected void OnCollisionEnter(Collision collision)
    {
        if(!m_Active)
        {
            return;
        }
        var bot = collision.collider.GetComponent<BaseAIController>();
        if (bot)
        {
            if (bot.instanceID != m_BulletOwnerID)
            {
                Health health = collision.gameObject.GetComponent<Health>();
                if (health)
                {
                    health.DoDamage(m_Damage);
                }
            }
        }
        else
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health)
            {
                health.DoDamage(m_Damage);
            }
        }

        Impact();
        m_Active = false;
    }

    protected void Impact()
    {
        Explode();
        Destroy(gameObject);
    }


    protected virtual void Explode()
    {
    }
}
