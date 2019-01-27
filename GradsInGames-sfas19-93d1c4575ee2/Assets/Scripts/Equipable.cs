using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable : MonoBehaviour
{
    GameObject m_Dad;
    Transform m_DadTransform;

    [SerializeField]
    Transform m_BulletSpawnPoint;

    [SerializeField]
    float m_RotationOffset;

    bool m_ChangeHands;

    public string m_ItemName;

    public void PickUp(PlayerController player)
    {
        Debug.Log("picked up");

        m_Dad = player.gameObject;
        m_DadTransform = player.transform;
        GetComponent<Collider>().enabled = false;
        GetComponent<GunLogic>().m_GunOwnerID = player.GetInstanceID();
    }

    public void PickUp(BaseAIController bot)
    {
        Debug.Log("picked up");

        m_Dad = bot.gameObject;
        m_DadTransform = bot.transform;
        GetComponent<Collider>().enabled = false;
        GetComponent<GunLogic>().m_GunOwnerID = bot.GetInstanceID();
    }

    public void Drop()
    {
        if (m_Dad)
        {
            if (m_Dad.GetComponent<BaseAIController>())
            {
                GetComponent<GunLogic>().ResetGun();
            }
        }      
        m_Dad = null;
        m_DadTransform = null;
        GetComponent<Collider>().enabled = true;
        GetComponent<GunLogic>().m_GunOwnerID = 0;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (m_Dad)
        {
            if(m_ChangeHands)
            {
                transform.position = m_DadTransform.position - m_DadTransform.right * 0.75f + m_DadTransform.forward / 2f;
            }
            else
            {
                transform.position = m_DadTransform.position + m_DadTransform.right*0.75f + m_DadTransform.forward/2f;
            }
            if (m_Dad.tag == "Player")
            {
                RotateTowardsMouse();
            }
            else
            {
                // transform.rotation = m_Dad.transform.rotation;
                m_BulletSpawnPoint.rotation = m_Dad.transform.rotation;
            }
        }
        else
        {
            Drop();
        }
    }

    public void RotateTowardsMouse()
    {
        Vector3 mousePosInScreenSpace = Input.mousePosition;
        Vector3 playerPosInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 directionInScreenSpace = mousePosInScreenSpace - playerPosInScreenSpace;

        float angle = Mathf.Atan2(directionInScreenSpace.y, directionInScreenSpace.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + m_RotationOffset, Vector3.up);
        m_BulletSpawnPoint.rotation = Quaternion.AngleAxis(-angle + 90, Vector3.up);
    }

    public bool AmIEquiped()
    {
        return m_Dad;
    }

    public void UpdateRotation(Quaternion lookRotation)
    {
        transform.rotation = lookRotation*Quaternion.AngleAxis(m_RotationOffset - 90, Vector3.up);
    }

    public void ToggleHand()
    {
        m_ChangeHands = !m_ChangeHands;
    }
}
