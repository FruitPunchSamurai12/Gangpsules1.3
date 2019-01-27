using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HubRooms
{
    kitchen,
    livingroom,
    bathroom,
    playroom,
    myroom
}


public class Hub : MonoBehaviour {

    public GameObject m_Player;
    public Transform m_PlayerSpawnLocation;
    public UIManager m_UIManager;
    public NavMeshSurface m_NavMesh;
    public GameObject johhny;
    public HubRooms whereIsJohhny;

    GameObject playerReference;
    

    public NPCDialogue[] kitchenDialogues;
    public NPCDialogue[] bathroomDialogues;
    public NPCDialogue[] livingroomDialogues;
    public NPCDialogue[] playroomDialogues;
    int kitchenIndex = 0;
    int bathroomIndex = 0;
    int livingroomIndex = 1;
    int playroomIndex = 0;
    

    private void Awake()
    {
         m_NavMesh.BuildNavMesh();
    }

    // Use this for initialization
    void Start () {
        if (!playerReference)
        {
            playerReference = Instantiate(m_Player, m_PlayerSpawnLocation.position, Quaternion.identity, transform);
        }
        else
        {
            playerReference.transform.position = m_PlayerSpawnLocation.position;
        }
        Camera cam = FindObjectOfType<Camera>();
        if (cam)
        {
            cam.GetComponent<CameraFollow>().m_PlayerTransform = playerReference.transform;
        }
        m_UIManager.UpdateArcadeUI();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeJohhnyDialogue()
    {
        ChangeJohhnyDialogue(whereIsJohhny);
    }

    public void ChangeJohhnyDialogue(HubRooms roomType)
    {
        switch(roomType)
        {
            case HubRooms.bathroom:
                ActuallyChangeJohhnyDialogue(bathroomDialogues[bathroomIndex]);
                bathroomIndex++;
                if(bathroomIndex >= bathroomDialogues.Length)
                {
                    bathroomIndex = 0;
                }
                whereIsJohhny = HubRooms.bathroom;
                break;
            case HubRooms.kitchen:
                ActuallyChangeJohhnyDialogue(kitchenDialogues[kitchenIndex]);
                kitchenIndex++;
                if (kitchenIndex >= kitchenDialogues.Length)
                {
                    kitchenIndex = 0;
                }
                whereIsJohhny = HubRooms.kitchen;
                break;
            case HubRooms.livingroom:
                ActuallyChangeJohhnyDialogue(livingroomDialogues[livingroomIndex]);
                livingroomIndex++;
                if (livingroomIndex >= livingroomDialogues.Length)
                {
                    livingroomIndex = 0;
                }
                whereIsJohhny = HubRooms.livingroom;
                break;
            case HubRooms.playroom:
                ActuallyChangeJohhnyDialogue(playroomDialogues[playroomIndex]);
                playroomIndex++;
                if (playroomIndex >= playroomDialogues.Length)
                {
                    playroomIndex = 0;
                }
                whereIsJohhny = HubRooms.playroom;
                break;
        }
    }

    void ActuallyChangeJohhnyDialogue(NPCDialogue dialogue)
    {
        johhny.GetComponent<NeutralAIController>().ChangeDialogue(dialogue);
    }
}
