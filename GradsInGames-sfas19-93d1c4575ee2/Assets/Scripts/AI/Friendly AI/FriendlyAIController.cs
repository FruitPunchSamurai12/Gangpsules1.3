using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FriendlyAIController : BaseAIController {

    public string m_CharacterName = "Johhny";

    public NPCDialogue[] m_Dialogue;
    public int m_DialogueIndex = 0;
    public bool m_IsTalking = false;
    UIManager m_UIManager;
    PlayerController m_PlayerController;
    public float m_RespawnTime = 10f;
    Transform m_PlayerTransform;
    public Vector3 m_Target;
    public BaseAIController m_TargetController;
    public Vector3 m_SpawnLocation;
    RaycastHit hit;

    [SerializeField]
    GameObject m_MeleeWeapon;

    public bool engagingTarget = false;
    // --------------------------------------------------------------
    public NPCDialogue GetDialogue()
    {
        NPCDialogue dialogue = m_Dialogue[m_DialogueIndex];
        m_DialogueIndex++;
        if (m_DialogueIndex >= m_Dialogue.Length)
        {
            m_DialogueIndex = 0;
        }
        return dialogue;
    }

    public void ChangeDialogue(NPCDialogue dialogue)
    {
        m_Dialogue[0] = dialogue;
    }

    public void ChangeDialogue(NPCDialogue dialogue,int index)
    {
        if (index < m_Dialogue.Length)
        {
            m_Dialogue[index] = dialogue;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_UIManager = FindObjectOfType<UIManager>();
        // Get Player information
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        while (i < player.Length)
        {
            if (player[i].GetComponent<PlayerController>())
            {
                m_PlayerController = player[i].GetComponent<PlayerController>();
                m_PlayerTransform = player[i].transform;
                break;
            }
            i++;
        }
        m_MeleeWeapon.SetActive(false);
        m_SpawnLocation = transform.position;
    }


    private void Update()
    {
       m_State.UpdateState(this);
    }

    ///  ---    FRIENDLY AI ACTIONS     ----    ///

    public override void IddleLogic()
    {
        StopMoving();
        Vector3 mousePosInScreenSpace = Input.mousePosition;
        Vector3 playerPosInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 directionInScreenSpace = mousePosInScreenSpace - playerPosInScreenSpace;
        float angle = Mathf.Atan2(directionInScreenSpace.y, directionInScreenSpace.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90.0f, Vector3.up);
        if (m_Weapon)
        {
            m_Weapon.GetComponent<Equipable>().RotateTowardsMouse();
        }
    }

    public override void Talk()
    {
        StopMoving();
        transform.LookAt(m_PlayerTransform.position);
        if (m_Weapon)
        {
            m_Weapon.transform.LookAt(m_PlayerTransform.position);
        }
    }


    public override void LookAtTarget()
    {
        //StopMoving();
        engagingTarget = true;
        ApplyDirection(m_Target);
        m_Strafe = true;
        transform.LookAt(m_Target);
        if (m_Weapon)
        {
            m_Weapon.transform.LookAt(m_Target);
        }
        m_LocationToMoveTo = m_Target;
    }

    public override void AttackLogic()
    {

        engagingTarget = true;
        if (m_Weapon)
        {
            StopMoving();
            Quaternion lookRotation = Quaternion.LookRotation(m_Target - m_BulletSpawningPosition.position);
            transform.LookAt(m_Target);
            m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);
            m_LocationToMoveTo = m_Target;
            m_Weapon.GetComponent<GunLogic>().Fire();
        }
        else
        {
            m_MeleeWeapon.SetActive(true);
            Quaternion lookRotation = Quaternion.LookRotation(m_Target - transform.position);
            transform.LookAt(m_Target);
            m_LocationToMoveTo = m_Target;
        }
    }

    public override void RunAndGunLogic()
    {
        ApplyDirection(m_LocationToMoveTo);
        engagingTarget = true;
        //m_Strafe = true;
        if (m_Weapon)
        {
            Quaternion lookRotation = Quaternion.LookRotation(m_Target - m_BulletSpawningPosition.position);
            transform.LookAt(m_Target);
            m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);
            m_Weapon.GetComponent<GunLogic>().Fire();
        }
        else
        {
            m_MeleeWeapon.SetActive(true);
            Quaternion lookRotation = Quaternion.LookRotation(m_Target - transform.position);
            transform.LookAt(m_Target);
            m_LocationToMoveTo = m_Target;
        }
    }

    public override void ChaseLogic()
    {
        ApplyDirection(m_Target);
        engagingTarget = false;
        m_Strafe = false;
    }

    public override void FollowPlayer()
    {
        m_LocationToMoveTo = m_PlayerTransform.position;
        engagingTarget = false;
        ApplyDirection(m_LocationToMoveTo);
    }

    public override void MoveAroundLogic()
    {
        if (ReachedDestination())
        {
            m_LocationToMoveTo = new Vector3(m_PlayerTransform.position.x + Random.Range(-4, 4), m_PlayerTransform.position.y, m_PlayerTransform.position.z + Random.Range(-4, 4));
        }
        ApplyDirection(m_LocationToMoveTo);
    }

    public override void HideLogic()
    {
       if(m_TargetController)
        {
            RunAndGunLogic();
        }
       else
        {
            MovementLogic();
        }
    }


    public override void Die()
    {
        m_StateTime += Time.deltaTime;
        if (m_StateTime > m_RespawnTime)
        {
            if (m_PlayerController.IsPlayerAlive())
            {
                transform.position = m_SpawnLocation;
                Revive();
            }
        }
        else
        {
            m_IsAlive = false;
            m_Agent.enabled = false;
            var collider = GetComponent<CapsuleCollider>();
            collider.enabled = false;
            if (m_Weapon)
            {
                m_Weapon.GetComponent<Equipable>().Drop();
                m_Weapon = null;
            }

            if (m_StateTime <= 0.25f)
            {
                transform.Rotate(0f, 100f * Time.deltaTime, 0f);
                transform.Rotate(0f, 0f, 200f * Time.deltaTime);
            }
            else if (m_StateTime <= 1f)
            {
                transform.Rotate(0f, 200f * Time.deltaTime, 0f);
                transform.Rotate(0f, 0f, 200f * Time.deltaTime);
                transform.position += new Vector3(0f, -2f * Time.deltaTime, 0f);
            }
            else if (m_StateTime <= 3f)
            {
                transform.position += new Vector3(0f, -10f * Time.deltaTime, 0f);
            }
        }
    }

    ///     ---     FRIENDLY AI DECISIONS   ---     ///

    public override bool AmITalking()
    {
        if (!m_UIManager.talking)
        {
            m_IsTalking = false;
            return false;
        }
        else
        {
            return m_IsTalking;
        }
    }

    public override bool TooCloseToPlayer()
    {
        float distance = Vector3.Distance(transform.position, m_PlayerTransform.position);
        return distance < 5f;
    }

    public override bool TooFarFromPlayer()
    {
        float distance = Vector3.Distance(transform.position, m_PlayerTransform.position);
        return distance > 15f;
    }

    public override bool SeeTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_SightRange);
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
                    if (Physics.SphereCast(transform.position + Vector3.up * m_CharacterController.height * 3 / 4, m_CharacterController.radius / 8f, (hitColliders[i].transform.position - transform.position).normalized, out hit, m_SightRange))
                    {
                        if (hit.collider.tag != "Enemy")
                        {
                            Debug.Log("i'm seeing " + hit.collider.name);
                            return false;
                        }
                        else
                        {                         
                            m_Target = hitColliders[i].transform.position;
                            m_TargetController = hitColliders[i].GetComponent<BaseAIController>();
                            return true;
                        }
                    }
                    m_Target = hitColliders[i].transform.position;
                    m_TargetController = hitColliders[i].GetComponent<BaseAIController>();
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

    public override bool HearTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_SightRange);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Enemy")
            {
                float distance = Vector3.Distance(hitColliders[i].transform.position, transform.position);
                if (distance < 4f)
                {
                     m_Target = hitColliders[i].transform.position;
                    m_TargetController = hitColliders[i].GetComponent<BaseAIController>();
                    return true;
                }
                else if (distance < m_SightRange && hitColliders[i].GetComponent<BaseAIController>().IsFiring())
                {
                    m_Target = hitColliders[i].transform.position;
                    m_TargetController = hitColliders[i].GetComponent<BaseAIController>();
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

    public override bool FindWeapon()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius * 10);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Equipable")
            {
                Equipable equipable = hitColliders[i].GetComponent<Equipable>();
                if (equipable)
                {
                    Debug.Log("i found a" + equipable.name);
                    GunLogic weapon = hitColliders[i].GetComponent<GunLogic>();
                    if (weapon)
                    {
                        Debug.Log("i found a" + weapon.name);
                        if (!IsMyGunBetter(weapon))
                        {
                            m_LocationToMoveTo = hitColliders[i].transform.position;
                            return true;
                        }
                    }
                }
            }
            i++;
        }
        return false;
    }


    public override bool FindPlaceToHide()
    {
        if(!m_TargetController)
        {
            return false;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius * 15f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Arcade Obstacle")
            {
                var targetWeapon = m_TargetController.GetWeapon();
                Vector3 targetBulletSpawningPosition;
                if (targetWeapon)
                {
                    targetBulletSpawningPosition = targetWeapon.GetComponent<GunLogic>().m_BulletSpawnPoint.position;
                }
                else
                {
                    targetBulletSpawningPosition = m_Target;
                }
                float distance = Vector3.Distance(targetBulletSpawningPosition, hitColliders[i].transform.position);
                if (Physics.Raycast(targetBulletSpawningPosition, (targetBulletSpawningPosition - hitColliders[i].transform.position).normalized, out hit, distance))
                {
                    Debug.Log("hiding near " + hitColliders[i].name);
                    m_LocationToMoveTo = hitColliders[i].transform.position - (targetBulletSpawningPosition - hitColliders[i].transform.position).normalized * hitColliders[i].bounds.size.magnitude / 2;
                    return true;
                }
            }
            i++;
        }
        return false;
    }

    public override bool CanAttackTarget()
    {
        if (!m_TargetController)
        {
            return false;
        }
        float distance = Vector3.Distance(m_Target, transform.position);
        if(!m_Weapon)
        {
            return distance < 3f;
        }
        RaycastHit hit;
        if (Physics.SphereCast(m_BulletSpawningPosition.position, m_CharacterController.radius / 8f, (m_Target - m_BulletSpawningPosition.position).normalized, out hit, distance))
        {
            if (hit.collider.tag != "Enemy")
            {
                Debug.Log("moving towards target");
                Debug.Log(hit.collider.name);
                return false;
            }
            else
            {
                Debug.Log(hit.collider.name);
                Debug.Log("target hit with raycast");
                return true;
            }
        }
        return true;
    }

    public override bool AmICovered()
    {
        if (!m_TargetController)
        {
            return true;
        }
        var targetWeapon = m_TargetController.GetWeapon();
        Vector3 targetBulletSpawningPosition;
        if (targetWeapon)
        {
            targetBulletSpawningPosition = targetWeapon.GetComponent<GunLogic>().m_BulletSpawnPoint.position;
        }
        else
        {
            targetBulletSpawningPosition = m_Target;
        }
        float distance = Vector3.Distance(targetBulletSpawningPosition, transform.position);
        if (Physics.Raycast(targetBulletSpawningPosition, (transform.position - targetBulletSpawningPosition).normalized, out hit, distance))
        {
            if (hit.collider.tag == tag)
            {
                return false;
            }
            else
            {
                Debug.Log(hit.collider.name + " is covering me");
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    public override bool CanCoverFire()
    {
        return (CanAttackTarget() && AmICovered());
    }

    public override bool AmIAlerted()
    {
        return m_PlayerController.AlertFriend(this);
    }

}
