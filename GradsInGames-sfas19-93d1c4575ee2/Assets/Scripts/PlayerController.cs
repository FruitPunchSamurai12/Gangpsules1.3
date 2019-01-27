using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

    // The character's running speed
    [SerializeField]
    float m_RunSpeed = 5.0f;

    // The gravity strength
    [SerializeField]
    float m_Gravity = 60.0f;

    // The maximum speed the character can fall
    [SerializeField]
    float m_MaxFallSpeed = 20.0f;

    // The character's jump height
    [SerializeField]
    float m_JumpHeight = 6.0f;

    //The player's weapon
    [SerializeField]
    GameObject m_Weapon;

    [SerializeField]
    GameObject m_MeleeWeapon;

    GameObject m_ObjectInFront;
    GameObject m_ThrowableItem;
    // --------------------------------------------------------------

    // The charactercontroller of the player
    CharacterController m_CharacterController;

    // The current movement direction in x & z.
    Vector3 m_MovementDirection = Vector3.zero;

    // The current movement speed
    float m_MovementSpeed = 0.0f;

    // The current vertical / falling speed
    float m_VerticalSpeed = 0.0f;

    // The current movement offset
    Vector3 m_CurrentMovementOffset = Vector3.zero;

    // The starting position of the player
    public Vector3 m_SpawningPosition = Vector3.zero;

    // Whether the player is alive or not
    bool m_IsAlive = true;

    bool m_HoldingGun;
    Throwable item = null;

    float m_DeathTime = 0f;

    // The time it takes to respawn
    const float MAX_RESPAWN_TIME = 1.0f;
    float m_RespawnTime = MAX_RESPAWN_TIME;

    // The force added to the player (used for knockbacks)
    Vector3 m_Force = Vector3.zero;

    UIManager m_UIManager;

    public bool m_DialogueHasControl = false;
    public bool m_IsDrowning = false;

    // --------------------------------------------------------------

    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();      
    }

    // Use this for initialization
    void Start()
    {
        m_SpawningPosition = transform.position;
        m_UIManager = FindObjectOfType<UIManager>();

        if (m_Weapon)
        {           
            UpdateUI();            
        }
        else
        {
            m_HoldingGun = false;
        }

        m_MeleeWeapon.gameObject.SetActive(false);

        // Update UI
      
    }

    public GameObject GetWeapon() { return m_Weapon; }

    public void DropWeapon()
    {
        if (m_Weapon)
        {
            m_Weapon.GetComponent<Equipable>().Drop();
            m_Weapon = null;
            m_HoldingGun = false;
        }
    }

    public void PickUpWeapon(Equipable w)
    {
        DropWeapon();
        w.PickUp(this);
        m_Weapon = w.gameObject;
        m_HoldingGun = true;
    }

    public void UpdateUI()
    {
        if (m_UIManager)
        {
            if (m_Weapon)
            {
                m_UIManager.SetAmmoText(m_Weapon.GetComponent<GunLogic>().m_BulletAmmo);
            }
            else
            {
                m_UIManager.SetAmmoText();
            }
            m_UIManager.SetHealth(GetComponent<Health>().m_Health);
            UpdateWhatIsInFrontUI();
        }
    }

    void Jump()
    {
        m_VerticalSpeed = Mathf.Sqrt(m_JumpHeight * m_Gravity);
    }

    void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_Gravity * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeed);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeed);
    }

    void UpdateMovementState()
    {
        if (m_UIManager.talking)
        {
            return;
        }
        // Get Player's movement input and determine direction and set run speed
        float horizontalInput = Input.GetAxisRaw("Horizontal_P1");
        float verticalInput = Input.GetAxisRaw("Vertical_P1");

        m_MovementDirection = new Vector3(horizontalInput, 0, verticalInput);
        m_MovementSpeed = m_RunSpeed;
    }

    void UpdateJumpState()
    {
        if (m_UIManager.talking)
        {
            return;
        }

        // Character can jump when standing on the ground
        if (Input.GetButtonDown("Jump_P1") && m_CharacterController.isGrounded)
        {
            Jump();
        }
    }

    void UpdateWeaponState()
    {
        if (m_UIManager.talking )
        {
            return;
        }
        if (Input.GetButton("Fire1"))
        {
            if (item)
            {
                ThrowItem();
                return;
            }
            else if (!m_HoldingGun)
            {
                m_MeleeWeapon.SetActive(true);
                return;
            }
            else if (m_Weapon.GetComponent<GunLogic>().m_BulletAmmo <= 0)
            {
                DropWeapon();
                m_MeleeWeapon.SetActive(true);
                return;

            }
            else
            {
                m_Weapon.GetComponent<GunLogic>().Shooting();
            }
        }       
    }

    public bool IsGunFiring()
    {
        if(!m_Weapon)
        {
            return false;
        }
        return m_Weapon.GetComponent<GunLogic>().IsGunFiring();
    }

    public bool AlertFriend(FriendlyAIController friend)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Enemy")
            {
                Vector3 targetDir = hitColliders[i].transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                //if the target 
                if (angle < 45f)
                {

                    RaycastHit hit;
                    if (Physics.SphereCast(transform.position + Vector3.up * m_CharacterController.height * 3 / 4, m_CharacterController.radius / 8f, (hitColliders[i].transform.position - transform.position).normalized, out hit, 15f))
                    {
                        if (hit.collider.tag != "Enemy")
                        {
                            return false;
                        }
                        else
                        {
                            friend.m_Target = hitColliders[i].transform.position;
                            friend.m_TargetController = hitColliders[i].GetComponent<BaseAIController>();
                            return true;
                        }
                    }
                    friend.m_Target = hitColliders[i].transform.position;
                    friend.m_TargetController = hitColliders[i].GetComponent<BaseAIController>();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            i++;
        }
        return false;
    }


    void ThrowItem()
    {

            item.Throw();
            item = null;
            if (m_Weapon)
            {
                m_Weapon.SetActive(true);
            }

    }

    void CheckWhatIsInFront()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, m_CharacterController.radius*2f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Player")
            {
                i++; //Comment this out to stuck the game in an infinite loop everytime you press e
                continue;
            }
            if (hitColliders[i].tag == "Neutral" || hitColliders[i].tag == "Friend" || hitColliders[i].tag == "Throwable" || hitColliders[i].tag == "Equipable" || hitColliders[i].tag == "Change Level")
            {
                m_ObjectInFront = hitColliders[i].gameObject;
                UpdateWhatIsInFrontUI();
                return;           
            }
            i++;                       
        }
        m_ObjectInFront = null;
    }

    void UpdateWhatIsInFrontUI()
    {
   
        if(!m_ObjectInFront)
        {
            m_UIManager.UpdateObjectInFront("", false);
        }
        else if (m_ObjectInFront.tag == "Neutral")
        {
            m_UIManager.UpdateObjectInFront("Talk to " + m_ObjectInFront.name, true);
        }
        else if (m_ObjectInFront.tag == "Friend")
        {
            if(m_ObjectInFront.GetComponent<FriendlyAIController>().engagingTarget)
            {
                m_UIManager.UpdateObjectInFront("", false);
            }
            else
            {
                m_UIManager.UpdateObjectInFront("Talk to " + m_ObjectInFront.GetComponent<FriendlyAIController>().m_CharacterName, true);
            }          
        }
        else if (m_ObjectInFront.tag == "Throwable")
        {
            m_UIManager.UpdateObjectInFront("Pick up " + m_ObjectInFront.GetComponent<Throwable>().m_ItemName, true);
        }
        else if (m_ObjectInFront.tag == "Equipable")
        {
            m_UIManager.UpdateObjectInFront("Equip " + m_ObjectInFront.GetComponent<Equipable>().m_ItemName, true);
        }
        else if (m_ObjectInFront.tag == "Change Level")
        {
            m_UIManager.UpdateObjectInFront(m_ObjectInFront.GetComponent<ChangeLevelTrigger>().m_Message, true);
        }
        else
        {
            m_UIManager.UpdateObjectInFront("",false);
        }
    }

    void Interact()
    {
        if(!m_ObjectInFront)
        {
            return;
        }
        if (m_ObjectInFront.tag == "Neutral")
        {
            NPCDialogue dialogue = m_ObjectInFront.GetComponent<NeutralAIController>().GetDialogue();
            if (dialogue)
            {
                m_MovementDirection = Vector3.zero;
                m_MovementSpeed = 0f;
                m_UIManager.TalkToNPC(dialogue);
                m_ObjectInFront.GetComponent<NeutralAIController>().m_IsTalking = true;
                return;
            }
        }
        else if (m_ObjectInFront.tag == "Friend")
        {
            if (!m_ObjectInFront.GetComponent<FriendlyAIController>().engagingTarget)
            {
                NPCDialogue dialogue = m_ObjectInFront.GetComponent<FriendlyAIController>().GetDialogue();
                if (dialogue)
                {
                    m_MovementDirection = Vector3.zero;
                    m_MovementSpeed = 0f;
                    m_UIManager.TalkToNPC(dialogue);
                    m_ObjectInFront.GetComponent<FriendlyAIController>().m_IsTalking = true;
                    return;
                }
            }
        }
        else if (m_ObjectInFront.tag == "Throwable")
        {
            if (item)
            {
                return;
            }
            Throwable throwable = m_ObjectInFront.GetComponent<Throwable>();
            if (throwable)
            {
                throwable.PickUp(this);
                item = throwable;
                if (m_Weapon)
                {
                    m_Weapon.SetActive(false);
                }
                return;
            }
        }
        else if (m_ObjectInFront.tag == "Equipable")
        {
            Equipable equipable = m_ObjectInFront.GetComponent<Equipable>();
            if (equipable)
            {
                PickUpWeapon(equipable);
            }
            return;
        }
        else if (m_ObjectInFront.tag == "Change Level")
        {
            m_ObjectInFront.GetComponent<ChangeLevelTrigger>().ChangeLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is dead update the respawn timer and exit update loop
        if(!m_IsAlive)
        {
            //UpdateRespawnTime();
            DeathAnimation();
            return;
        }

        //Check if alive
        CheckHealth();

        if(m_IsDrowning)
        {
            DrowningAnimation();
            return;
        }


        if (Input.GetButtonDown("Interact"))
        {
            if(m_UIManager.talking)
            {
                if (!m_DialogueHasControl)
                {
                    m_UIManager.ManageDialogues();
                }
            }
            else
            {
                Interact();               
            }           
        }

        CheckWhatIsInFront();

        ToggleHands();

        UpdateUI();

        // Update movement input
        UpdateMovementState();

        // Update jumping input and apply gravity
        UpdateJumpState();
        ApplyGravity();

        //Update Weapon Input
        UpdateWeaponState();

        

        // Calculate actual motion
        m_CurrentMovementOffset = (m_MovementDirection * m_MovementSpeed + m_Force  + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;

        m_Force *= 0.95f;

        // Move character
        m_CharacterController.Move(m_CurrentMovementOffset);

        // Rotate the character towards the mouse cursor
        RotateCharacterTowardsMouseCursor();
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void RotateCharacter(Vector3 movementDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation)
        {
            transform.rotation = lookRotation;
        }
    }

    void RotateCharacterTowardsMouseCursor()
    {
        if (m_UIManager.talking)
        {
            return;
        }
        Vector3 mousePosInScreenSpace = Input.mousePosition;
        Vector3 playerPosInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 directionInScreenSpace = mousePosInScreenSpace - playerPosInScreenSpace;

        float angle = Mathf.Atan2(directionInScreenSpace.y, directionInScreenSpace.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90.0f, Vector3.up);
    }

    public void Die()
    {
        m_IsAlive = false;
        m_RespawnTime = MAX_RESPAWN_TIME;
    }

    public void AITalkToPlayer(GameObject ai)
    {
        NPCDialogue dialogue = ai.GetComponent<NeutralAIController>().GetDialogue();
        if (dialogue)
        {
            m_MovementDirection = Vector3.zero;
            m_MovementSpeed = 0f;
            transform.LookAt(ai.transform.position);
            if (m_Weapon)
            {
                m_Weapon.GetComponent<Equipable>().UpdateRotation(transform.rotation);
            }
            m_UIManager.TalkToNPC(dialogue);
            ai.GetComponent<NeutralAIController>().m_IsTalking = true;          
        }
    }

    void UpdateRespawnTime()
    {
        m_RespawnTime -= Time.deltaTime;
        if (m_RespawnTime < 0.0f)
        {
            Respawn();
        }
    }

    void CheckHealth()
    {
        Health h = GetComponent<Health>();
        if(!h.IsAlive())
        {
            m_IsAlive = false;
            m_UIManager.ActivateDeathScene();
        }

    }

    void DeathAnimation()
    {
        m_DeathTime += Time.deltaTime;
        var collider = GetComponent<CapsuleCollider>();
        collider.enabled = false;
        if (m_Weapon)
        {
            DropWeapon();
        }
        if (m_DeathTime <= 0.25f)
        {
            transform.Rotate(0f, 100f * Time.deltaTime, 0f);
            transform.Rotate(0f, 0f, 200f * Time.deltaTime);
        }
        else if (m_DeathTime <= 1f)
        {
            transform.Rotate(0f, 200f * Time.deltaTime, 0f);
            transform.Rotate(0f, 0f, 200f * Time.deltaTime);
            transform.position += new Vector3(0f, -2f * Time.deltaTime, 0f);
        }
        else if (m_DeathTime <= 3f)
        {
            transform.position += new Vector3(0f, -10f * Time.deltaTime, 0f);
        }                             
    }

    public void DrowningAnimation()
    {
        m_DeathTime += Time.deltaTime;
        var collider = GetComponent<CapsuleCollider>();
        collider.enabled = false;
        if (m_Weapon)
        {
            DropWeapon();
        }
        if (m_DeathTime <= 0.25f)
        {
            transform.Rotate(0f, 100f * Time.deltaTime, 0f);
            transform.Rotate(0f, 0f, 200f * Time.deltaTime);
        }
        else if (m_DeathTime <= 1f)
        {
            transform.Rotate(0f, 200f * Time.deltaTime, 0f);
            transform.Rotate(0f, 0f, 200f * Time.deltaTime);
            transform.position += new Vector3(0f, -2f * Time.deltaTime, 0f);
        }
        else if (m_DeathTime <= 3f)
        {
            transform.position += new Vector3(0f, -10f * Time.deltaTime, 0f);
        }
        else
        {
            m_DeathTime = 0;
            m_IsDrowning = false;
            transform.rotation = Quaternion.identity;
            transform.position = m_SpawningPosition;
            collider.enabled = true;            
        }
    }

    void Respawn()
    {
        m_IsAlive = true;
        transform.position = m_SpawningPosition;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    public void AddForce(Vector3 force)
    {
        m_Force += force;
    }
   
    void ToggleHands()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(m_Weapon)
            {
                m_Weapon.GetComponent<Equipable>().ToggleHand();
            }
        }
    }

    public bool IsPlayerAlive()
    {
        return m_IsAlive;
    }
}
