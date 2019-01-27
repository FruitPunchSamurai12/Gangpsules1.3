using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour {

    public HubRooms room1;
    public HubRooms room2;
    public Hub m_Hub;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Johhny")
        {
            if(room1 == m_Hub.whereIsJohhny)
            {
                m_Hub.ChangeJohhnyDialogue(room2);
            }
            else if(room2 == m_Hub.whereIsJohhny)
            {
                m_Hub.ChangeJohhnyDialogue(room1);
            }
        }
    }
}
