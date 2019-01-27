using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum throwableStates
{

    iddle,
    pickedUp,
    thrown
}

public class Throwable : MonoBehaviour {

    public string m_ItemName;

    [SerializeField]
    float m_Speed;

    [SerializeField]
    int m_Damage;

    bool m_Active = true;

    public throwableStates m_State = throwableStates.iddle;

    Transform m_DadTransform;


    public void PickUp(PlayerController player)
    {
        m_State = throwableStates.pickedUp;
        m_DadTransform = player.transform;
        GetComponent<Collider>().enabled = false;
    }

    public void PickUp(NeutralAIController bot)
    {
        m_State = throwableStates.pickedUp;
        m_DadTransform = bot.transform;
        GetComponent<Collider>().enabled = false;
    }

    public void Drop(Vector3 pos)
    {
        m_State = throwableStates.iddle;
        GetComponent<Collider>().enabled = true;
        m_DadTransform = null;
        transform.position = pos;
    }

    public void Throw()
    {
        m_State = throwableStates.thrown;
        GetComponent<Collider>().enabled = true;
        GetComponent<MeshCollider>().convex = true;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = m_DadTransform.forward * m_Speed;
       
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!m_Active)
        {
            return;
        }
        if (m_State != throwableStates.thrown)
        {
            return;
        }
        

        Health health = collision.gameObject.GetComponent<Health>();
        if (health)
        {
            health.DoDamage(m_Damage);
        }

        Impact();
        m_Active = false;
    }

    void Impact()
    {
        Destroy(gameObject);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(m_State == throwableStates.pickedUp)
        {
            transform.position = m_DadTransform.position + m_DadTransform.right;
            
        }
        else if(m_State == throwableStates.thrown)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddTorque(transform.right*20f);
        }
	}
}
