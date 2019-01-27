using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum levelPhase
{
    phase1,
    phase2,
    phase3,
    phase4,
    phase5,
    phase6
}

public class Casino : MonoBehaviour {

    public bool m_LevelWon = false;

    public UIManager m_UIManager;

    public GameObject m_Player;
    public Transform m_PlayerSpawnLocation;
    GameObject playerReference;
    public PlayerController m_PlayerController;
    public GameObject m_Friend;
    public Transform m_FriendSpawnLocation;
    GameObject friendReference;
    public FriendlyAIController m_FriendController;

    [SerializeField] NavMeshSurface surface;
    [SerializeField] GameObject m_Camera;

    [SerializeField] GameObject[] immovableNPCs;
    int immovableNpcNumber;
    [SerializeField] GameObject[] NPCS;
    int npcNumber;
    [SerializeField] Transform runAwayPoint;
    [SerializeField] AIState runAwayState;

    [SerializeField] GameObject callum;

    [SerializeField] GameObject guard1;
    [SerializeField] GameObject guard2;
    [SerializeField] GameObject guard1Pistol;
    [SerializeField] GameObject guard2Pistol;

    [SerializeField] AIState guardsPhase2State;
    [SerializeField] Waypoint guardsWaypoint;

    [SerializeField] SpawnPoint leftSideSpawnPoint;
    [SerializeField] SpawnPoint rightSideSpawnPoint;
    [SerializeField] SpawnPoint doorLeftSideSpawnPoint;
    [SerializeField] SpawnPoint doorRightSideSpawnPoint;
    [SerializeField] SpawnPoint doorMiddleSpawnPoint;

    [SerializeField] GameObject mrFatpill;
    [SerializeField] SpawnPoint mrFatpillShotgun;
    [SerializeField] AIState mrFatpillPickUpShotgunState;
    [SerializeField] int mrFatpillHealth = 500;

    [SerializeField] NPCDialogue[] johhnyPhase2Dialogues;
    [SerializeField] NPCDialogue[] johhnyPhase5Dialogues;
    [SerializeField] NPCDialogue[] johhnyPhase6Dialogues;

    [SerializeField] GameObject[] supplies;

    [SerializeField] AudioClip battleClip;
    [SerializeField] AudioClip afterBattleClip;
    [SerializeField] AudioClip hubClip;


    public levelPhase m_Phase = levelPhase.phase1;

    private void Awake()
    {
        surface.BuildNavMesh();
    }
    // Use this for initialization
    void Start () {
        guard1.GetComponent<BaseAIController>().PickUpWeapon(guard1Pistol.GetComponent<Equipable>());
        guard2.GetComponent<BaseAIController>().PickUpWeapon(guard2Pistol.GetComponent<Equipable>());
        if (!playerReference)
        {
            playerReference = Instantiate(m_Player, m_PlayerSpawnLocation.position, Quaternion.identity, transform);
            m_PlayerController = playerReference.GetComponent<PlayerController>();
        }
        else
        {
            playerReference.transform.position = m_PlayerSpawnLocation.position;
            m_PlayerController = playerReference.GetComponent<PlayerController>();
        }
        Camera cam = FindObjectOfType<Camera>();
        if (cam)
        {
            cam.GetComponent<CameraFollow>().m_PlayerTransform = playerReference.transform;
        }
        if (!friendReference)
        {
            friendReference = Instantiate(m_Friend, m_FriendSpawnLocation.position, Quaternion.identity, transform);
            m_FriendController = friendReference.GetComponent<FriendlyAIController>();
            m_FriendController.m_SpawnLocation = m_FriendSpawnLocation.position;
        }
        else
        {
            friendReference.transform.position = m_FriendSpawnLocation.position;
            m_FriendController = friendReference.GetComponent<FriendlyAIController>();
            m_FriendController.m_SpawnLocation = m_FriendSpawnLocation.position;
        }
        m_UIManager.UpdateArcadeUI();
        npcNumber = NPCS.Length;
        immovableNpcNumber = immovableNPCs.Length;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_Phase == levelPhase.phase1)
        {
            Phase1();
        }
        else if (m_Phase == levelPhase.phase2)
        {
            Phase2();
        }
        else if (m_Phase == levelPhase.phase3)
        {
            Phase3();
        }
        else if (m_Phase == levelPhase.phase4)
        {
            Phase4();
        }
        else if (m_Phase == levelPhase.phase5)
        {
            Phase5();
        }
        else if (m_Phase == levelPhase.phase6)
        {
            Phase6();
        }
        TellNPCsWhereToRunAway();
    }


    void Phase1()
    {
        if (!mrFatpill.GetComponent<NeutralAIController>().m_IsTalking)
        {
            if (mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex == 4)
            {
                m_Phase = levelPhase.phase2;
                var music = FindObjectOfType<PlayMusic>();
                music.PlaySelectedMusic(battleClip);
                ChangeJohhnyDialogues();
            }
        }
        if(mrFatpill.GetComponent<NeutralAIController>().ShouldIHide() || guard1.GetComponent<NeutralAIController>().ShouldIHide() || guard2.GetComponent<NeutralAIController>().ShouldIHide())
        {
            EarlyPhase2();
        }
    }

    void Phase2()
    {
       
        if (!mrFatpill.GetComponent<NeutralAIController>().AmITalking())
        {
            m_Camera.GetComponent<CameraFollow>().m_CloseIn = false;
            
        }
      
            mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex = 6;
            if (!guard1 && !guard2)
            {
                mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex = 4;
                m_PlayerController.AITalkToPlayer(mrFatpill);
                m_Camera.GetComponent<CameraFollow>().CloseIn(mrFatpill.transform);
                m_Phase = levelPhase.phase3;
            }
            else
            {
                if (guard1)
                {
                    if (guard1.GetComponent<NeutralAIController>())
                    {
                        guard1.GetComponent<NeutralAIController>().DropWeapon();
                        Destroy(guard1.GetComponent<NeutralAIController>());
                        var ai1 = guard1.AddComponent<AIController>();
                        ai1.ChangeAIState(guardsPhase2State);
                        ai1.BuildAIController();
                        ai1.m_CurrentWaypoint = guardsWaypoint;
                        ai1.PickUpWeapon(guard1Pistol.GetComponent<Equipable>());
                    }
                }
                if (guard2)
                {
                    if (guard2.GetComponent<NeutralAIController>())
                    {
                        guard2.GetComponent<NeutralAIController>().DropWeapon();
                        Destroy(guard2.GetComponent<NeutralAIController>());
                        var ai2 = guard2.AddComponent<AIController>();
                        ai2.ChangeAIState(guardsPhase2State);
                        ai2.BuildAIController();
                        ai2.m_CurrentWaypoint = guardsWaypoint;
                        ai2.PickUpWeapon(guard2Pistol.GetComponent<Equipable>());
                    }
                }
            }
        
    }

    void Phase3()
    {
       
        mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex = 6;
        if (!mrFatpill.GetComponent<NeutralAIController>().m_IsTalking)
        {
            m_Camera.GetComponent<CameraFollow>().m_CloseIn = false;
            if (leftSideSpawnPoint.HasSpawned() && rightSideSpawnPoint.HasSpawned() && doorMiddleSpawnPoint.HasSpawned())
            {
                if (leftSideSpawnPoint.m_NumberOfActiveEnemies == 0 && rightSideSpawnPoint.m_NumberOfActiveEnemies == 0 && doorMiddleSpawnPoint.m_NumberOfActiveEnemies == 0)
                {
                    leftSideSpawnPoint.Spawn();
                    rightSideSpawnPoint.Spawn();
                    doorLeftSideSpawnPoint.Spawn();
                    doorRightSideSpawnPoint.Spawn();
                    m_Phase = levelPhase.phase4;
                }
            }
            else
            {
                if (!leftSideSpawnPoint.orderedToSpawn)
                {
                    leftSideSpawnPoint.orderedToSpawn = true;
                    leftSideSpawnPoint.Spawn();
                }
                if (!rightSideSpawnPoint.orderedToSpawn)
                {
                    rightSideSpawnPoint.orderedToSpawn = true;
                    rightSideSpawnPoint.Spawn();
                }
                if (!doorMiddleSpawnPoint.orderedToSpawn)
                {
                    doorMiddleSpawnPoint.orderedToSpawn = true;
                    doorMiddleSpawnPoint.Spawn();
                }
            }
        }
    }

    void Phase4()
    {
        
        mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex = 6;
        if (leftSideSpawnPoint.m_NumberOfActiveEnemies == 0 && rightSideSpawnPoint.m_NumberOfActiveEnemies == 0 && doorLeftSideSpawnPoint.m_NumberOfActiveEnemies == 0 && doorRightSideSpawnPoint.m_NumberOfActiveEnemies == 0)
        {
            mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex = 5;
            m_PlayerController.AITalkToPlayer(mrFatpill);
            m_Camera.GetComponent<CameraFollow>().CloseIn(mrFatpill.transform);
            TellNPCSToRunAway();
            m_Phase = levelPhase.phase5;
            ChangeJohhnyDialogues();
        }
    }

    void Phase5()
    {
       
        if (mrFatpill)
        {
            if (mrFatpill.GetComponent<NeutralAIController>())
            {
                if (!mrFatpill.GetComponent<NeutralAIController>().m_IsTalking)
                {
                    m_Camera.GetComponent<CameraFollow>().m_CloseIn = false;
                    leftSideSpawnPoint.DelayedSpawn(1f);
                    rightSideSpawnPoint.DelayedSpawn(1f);
                    doorLeftSideSpawnPoint.DelayedSpawn(1f);
                    doorRightSideSpawnPoint.DelayedSpawn(1f);
                    doorMiddleSpawnPoint.DelayedSpawn(1f);
                    mrFatpillShotgun.autoSpawn = true;
                    Destroy(mrFatpill.GetComponent<NeutralAIController>());
                    var ai = mrFatpill.AddComponent<MrFatpill>();
                    mrFatpill.GetComponent<Health>().m_Health = mrFatpillHealth;
                    mrFatpill.GetComponent<Health>().m_MaxHealth = mrFatpillHealth;
                    ai.ChangeAIState(mrFatpillPickUpShotgunState);
                    ai.BuildAIController();
                    ai.m_LocationToMoveTo = mrFatpillShotgun.transform.position;
                }
            }
        }
        if (leftSideSpawnPoint.m_NumberOfActiveEnemies == 0 && rightSideSpawnPoint.m_NumberOfActiveEnemies == 0 && doorLeftSideSpawnPoint.m_NumberOfActiveEnemies == 0 &&
                    doorRightSideSpawnPoint.m_NumberOfActiveEnemies == 0 && doorMiddleSpawnPoint.m_NumberOfActiveEnemies == 0 && !mrFatpill)
        {           
            m_Phase = levelPhase.phase6;
            var start = FindObjectOfType<StartOptions>();
            if (start)
            {
                start.menuSettingsData.musicLoopToChangeTo = afterBattleClip;
                start.PlayNewMusic();
            }
            ChangeJohhnyDialogues();
        }
    }

    void Phase6()
    {
        
        if (!callum.GetComponent<NeutralAIController>().m_IsTalking)
        {
            if(m_LevelWon)
            {
                StartOptions start = FindObjectOfType<StartOptions>();
                start.sceneToStart = 1;
                start.menuSettingsData.musicLoopToChangeTo = hubClip;
                start.StartButtonClicked();
                m_LevelWon = false;
            }
        }
    }

    void TellNPCsWhereToRunAway()
    {
        int i = 0;
        while(i<npcNumber)
        {
            if(!NPCS[i])
            {
                NPCS[i] = NPCS[npcNumber - 1];
                npcNumber--;              
                return;
            }
            if(NPCS[i].GetComponent<NeutralAIController>().m_Fleeing)
            {
                NPCS[i].GetComponent<NeutralAIController>().m_LocationToMoveTo = runAwayPoint.position;
                NPCS[i] = NPCS[npcNumber - 1];
                npcNumber--;
                return;
            }
            i++;
        }
        i = 0;
        while (i < immovableNpcNumber)
        {
            if (!immovableNPCs[i])
            {
                immovableNPCs[i] = immovableNPCs[immovableNpcNumber - 1];
                immovableNpcNumber--;
                return;
            }
            if (immovableNPCs[i].GetComponent<NeutralAIController>().ShouldIHide())
            {
                immovableNPCs[i].GetComponent<NeutralAIController>().BuildNeutralAIController();
                immovableNPCs[i].GetComponent<NeutralAIController>().ChangeAIState(runAwayState);
                immovableNPCs[i].GetComponent<NeutralAIController>().m_LocationToMoveTo = runAwayPoint.position;
                immovableNPCs[i].GetComponent<NeutralAIController>().levelScriptControl = false;
                immovableNPCs[i] = immovableNPCs[immovableNpcNumber - 1];
                immovableNpcNumber--;
                return;
            }
            i++;
        }
    }

    void TellNPCSToRunAway()
    {
        int i = 0;
        while (i < npcNumber)
        {
            if (NPCS[i])
            {
                NPCS[i].GetComponent<NeutralAIController>().ChangeAIState(runAwayState);
                NPCS[i].GetComponent<NeutralAIController>().m_LocationToMoveTo = runAwayPoint.position;
            }
            i++;
        }
        i = 0;
        while (i < immovableNpcNumber)
        {
            if (immovableNPCs[i])
            {
                immovableNPCs[i].GetComponent<NeutralAIController>().BuildNeutralAIController();
                immovableNPCs[i].GetComponent<NeutralAIController>().ChangeAIState(runAwayState);
                immovableNPCs[i].GetComponent<NeutralAIController>().m_LocationToMoveTo = runAwayPoint.position;
                immovableNPCs[i].GetComponent<NeutralAIController>().levelScriptControl = false;
            }
            i++;
        }
    }

    public void EarlyPhase2()
    {
        var music = FindObjectOfType<PlayMusic>();
        music.PlaySelectedMusic(battleClip);
        mrFatpill.GetComponent<NeutralAIController>().m_DialogueIndex = 6;
        m_PlayerController.AITalkToPlayer(mrFatpill);
        m_Camera.GetComponent<CameraFollow>().CloseIn(mrFatpill.transform);       
        m_Phase = levelPhase.phase2;
        ChangeJohhnyDialogues();
    }

    void ChangeJohhnyDialogues()
    {
        if(friendReference)
        {
            var johhny = friendReference.GetComponent<FriendlyAIController>();
            if (johhny)
            {
                johhny.m_DialogueIndex = 0;
                if (m_Phase == levelPhase.phase2)
                {

                    johhny.ChangeDialogue(johhnyPhase2Dialogues[0], 0);
                    johhny.ChangeDialogue(johhnyPhase2Dialogues[1], 1);
                    johhny.ChangeDialogue(johhnyPhase2Dialogues[2], 2);
                }
                else if (m_Phase == levelPhase.phase5)
                {

                    johhny.ChangeDialogue(johhnyPhase5Dialogues[0], 0);
                    johhny.ChangeDialogue(johhnyPhase5Dialogues[1], 1);
                    johhny.ChangeDialogue(johhnyPhase5Dialogues[2], 2);
                }
                else if (m_Phase == levelPhase.phase6)
                {

                    johhny.ChangeDialogue(johhnyPhase6Dialogues[0], 0);
                    johhny.ChangeDialogue(johhnyPhase6Dialogues[1], 1);
                    johhny.ChangeDialogue(johhnyPhase6Dialogues[2], 2);
                }
            }

        }
              
    }

    void PlayMusic(AudioClip clip)
    {
        var start = FindObjectOfType<StartOptions>();
        if (start)
        {
            start.menuSettingsData.musicLoopToChangeTo = clip;
            start.PlayNewMusic();
        }
    }

    public GameObject DropSupply()
    {
        int random = Random.Range(0, 100);
        if(random <50)
        {
            return supplies[0];
        }
        else if(random <80)
        {
            return supplies[1];
        }
        else
        {
            return supplies[2];
        }
    }

}
