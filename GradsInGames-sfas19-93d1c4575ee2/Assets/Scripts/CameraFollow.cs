using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The Camera Target
    public Transform m_PlayerTransform;
    public Transform m_TargetTransform;
    public Quaternion m_OriginalRotation;
    public bool m_CloseIn = false;

    //The long Y distance from the ground
    [SerializeField]
    float m_CameraLongDistanceY = 15.0f;
    // The long Z Distance from the Camera Target
    [SerializeField]
    float m_CameraLongDistanceZ = 15.0f;


    //The short Y distance from the ground
    [SerializeField]
    float m_CameraLShortDistanceY = 3.0f;
    // The short Z Distance from the Camera Target
    [SerializeField]
    float m_CameraShortDistanceZ = 10.0f;

    //The camera's moving speed
    [SerializeField]
    float m_Speed = 20f;


    Vector3 m_StartPosition;
    Vector3 m_EndPosition;
    public float m_Time = 0f;
    public float m_TimeLimit = 1.5f;

    // Use this for initialization
    void Start ()
    {
        m_OriginalRotation = transform.rotation;
        m_StartPosition = transform.position;
        if (m_PlayerTransform)
        {
            m_EndPosition = new Vector3(m_PlayerTransform.position.x, m_CameraLongDistanceY, m_PlayerTransform.position.z - m_CameraLongDistanceZ);
        }
        else
        {
            m_EndPosition = transform.position;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(m_CloseIn)
        {
            if(m_TargetTransform)
            {
                m_Time += Time.deltaTime;
                if (m_Time < m_TimeLimit*2f)
                {
                    transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, m_Time / (m_TimeLimit*2f));
                    Vector3 direction = m_TargetTransform.position - transform.position;
                    Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, m_Time / (m_TimeLimit * 2f));
                }
                else
                {
                    transform.position = new Vector3(m_PlayerTransform.position.x, m_CameraLShortDistanceY, m_PlayerTransform.position.z - m_CameraShortDistanceZ);
                    transform.LookAt(m_TargetTransform);
                }
            }
            else
            {
                m_CloseIn = false;              
            }
        }
        else
        {
            PanOut();
            transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, Time.deltaTime/m_TimeLimit);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_OriginalRotation, Time.deltaTime / m_TimeLimit);               
        }
        
	}

    public void CloseIn(Transform target)
    {
        m_Time = 0;
        m_CloseIn = true;
        m_TargetTransform = target;
        m_StartPosition = transform.position;
        m_EndPosition = new Vector3(m_PlayerTransform.position.x, m_CameraLShortDistanceY, m_PlayerTransform.position.z - m_CameraShortDistanceZ);
    }

    public void PanOut()
    {
        m_Time = 0;
        m_CloseIn = false;
        m_TargetTransform = null;
        m_StartPosition = transform.position;
        m_EndPosition = new Vector3(m_PlayerTransform.position.x, m_CameraLongDistanceY, m_PlayerTransform.position.z - m_CameraLongDistanceZ);
    }
}
