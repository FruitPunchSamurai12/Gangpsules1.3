using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum Targets
{
    player,
    friend
}


public class AIController : BaseAIController
{
    [SerializeField]
    Targets m_Target = Targets.player;



    // --------------------------------------------------------------
    public bool alertOnTarget = false;

   [SerializeField]
    PlayerController m_PlayerController;
    [SerializeField]
    Transform m_PlayerTransform;

    FriendlyAIController m_FriendController;
    Transform m_FriendTransform;
    bool isFriendDead = false;

    RaycastHit hit;

    public bool engagingTarget = false;
   
    // --------------------------------------------------------------



    // Use this for initialization
    void Start()
    {
        m_SightRange = 15.0f;

        if (m_Weapon)
        {
            m_SightRange = m_Weapon.GetComponent<GunLogic>().m_Range;
            m_BulletSpawningPosition = m_Weapon.GetComponent<GunLogic>().m_BulletSpawnPoint;
        }
        else
        {
            PickUpWeapon();
        }
        // Get Player information
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        while (i<player.Length)
        {
            if(player[i].GetComponent<PlayerController>())
            {
                m_PlayerController = player[i].GetComponent<PlayerController>();
                m_PlayerTransform = player[i].transform;
                break;
            }
            i++;
        }    
        GameObject[] friend = GameObject.FindGameObjectsWithTag("Friend");
        int j = 0;
        while (j < friend.Length)
        {
            if (friend[j].GetComponent<FriendlyAIController>())
            {
                m_FriendController = friend[j].GetComponent<FriendlyAIController>();
                m_FriendTransform = friend[j].transform;
            }
            j++;
        }

    } 

    public void BuildAIController()
    {
        m_CharacterController = GetComponent<CharacterController>();
        tag = "Enemy";
        m_Agent = GetComponent<NavMeshAgent>();
        m_Destination = transform.position;
        instanceID = GetInstanceID();
        m_SightRange = 15.0f;

        if (m_Weapon)
        {
            m_SightRange = m_Weapon.GetComponent<GunLogic>().m_Range;
            m_BulletSpawningPosition = m_Weapon.GetComponent<GunLogic>().m_BulletSpawnPoint;
        }
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
        GameObject[] friend = GameObject.FindGameObjectsWithTag("Friend");
        int j = 0;
        while (j < friend.Length)
        {
            if (friend[j].GetComponent<FriendlyAIController>())
            {
                m_FriendController = friend[j].GetComponent<FriendlyAIController>();
                m_FriendTransform = friend[j].transform;
            }
            j++;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // If the player is dead update the respawn timer and exit update loop
        /*if (!m_IsAlive)
        {
            return;
        }*/
        ToggleTarget();
        m_State.UpdateState(this);
    }

    //          AI DECISIONS            //
    public override bool IsTargetActive()
    {
        if (m_Target == Targets.player)
        {
          if(m_PlayerController)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (m_Target == Targets.friend)
        {
            if (m_FriendController)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public override bool TargetInRange()
    {
        return DistanceFromTarget() < m_SightRange;
    }

    public override bool SeeTarget()
    {
        Vector3 targetDir = TargetTransform().position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        float distance = DistanceFromTarget();

        //if the target 
        if (distance < m_SightRange && angle < 45f)
        {
            
            RaycastHit hit;
            if (Physics.SphereCast(transform.position + Vector3.up*m_CharacterController.height*3/4, m_CharacterController.radius / 8f, (TargetTransform().position - transform.position).normalized, out hit, distance))
            {
                if (hit.collider.tag != TargetTag())
                {
                    Debug.Log("i'm seeing " + hit.collider.name);
                    return false;
                }
                else
                {
                    m_LocationToMoveTo = TargetTransform().position;
                    return true;
                }
            }
            m_LocationToMoveTo = TargetTransform().position;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool HearTarget()
    {
        if (DistanceFromTarget() < 4f)
        {
            m_LocationToMoveTo = TargetTransform().position;
            return true;
        }
        else if (DistanceFromTarget() < m_SightRange && IsTargetFiring())
        {
            m_LocationToMoveTo = TargetTransform().position;
            return true;
        }
        else
        {
            return false;
        }
    }


    public override bool AmIAlerted()
    {
        if(alertOnTarget)
        {
            alertOnTarget = false;
            m_LocationToMoveTo = TargetTransform().position;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool FindPlaceToHide()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius *20f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Arcade Obstacle")
            {
                var targetWeapon = GetTargetWeapon();
                Vector3 targetBulletSpawningPosition;
                if (targetWeapon)
                {
                    targetBulletSpawningPosition = targetWeapon.GetComponent<GunLogic>().m_BulletSpawnPoint.position;
                } 
                else
                {
                    targetBulletSpawningPosition = TargetTransform().position;
                }
                float distance = Vector3.Distance(targetBulletSpawningPosition, hitColliders[i].transform.position);
                if (Physics.Raycast(targetBulletSpawningPosition, (targetBulletSpawningPosition - hitColliders[i].transform.position).normalized, out hit, distance))
                {
                    Debug.Log("hiding near " + hitColliders[i].name);
                    m_LocationToMoveTo = hitColliders[i].transform.position - (targetBulletSpawningPosition - hitColliders[i].transform.position).normalized * hitColliders[i].bounds.size.magnitude/2;
                    return true;
                }
            }
            i++;
        }
        return false;
    }

    public override bool CanAttackTarget()
    {
        if(!m_Weapon)
        {
            return false;
        }
        float distance = DistanceFromTarget();
        RaycastHit hit;
       // if (Physics.SphereCast(m_BulletSpawningPosition.position, m_CharacterController.radius/2f, (TargetTransform().position - m_BulletSpawningPosition.position).normalized, out hit, distance))
       if(Physics.SphereCast(m_BulletSpawningPosition.position,m_CharacterController.radius/8f, (TargetTransform().position - m_BulletSpawningPosition.position).normalized, out hit, distance))
        {
            if (hit.collider.tag != TargetTag())
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
        var targetWeapon = GetTargetWeapon();
        Vector3 targetBulletSpawningPosition;
        if (targetWeapon)
        {
            targetBulletSpawningPosition = targetWeapon.GetComponent<GunLogic>().m_BulletSpawnPoint.position;
        }
        else
        {
            targetBulletSpawningPosition = TargetTransform().position;
        }
        float distance = Vector3.Distance(targetBulletSpawningPosition, transform.position);
        if (Physics.Raycast(targetBulletSpawningPosition, (transform.position - targetBulletSpawningPosition).normalized, out hit, distance))
        {
            if(hit.collider.tag == tag)
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

    public override bool TooCloseToPlayer()
    {
        float distance = Vector3.Distance(transform.position, TargetTransform().position);
        return distance < 3f;
    }


    //          AI ACTIONS          //
    public override void ChaseLogic()
    {
        engagingTarget = false;
        ApplyDirection(m_LocationToMoveTo);
        m_Strafe = false;
    }

    public override void LookAtTarget()
    {
        //StopMoving();
        engagingTarget = true;
        ApplyDirection(m_LocationToMoveTo);
        m_Strafe = true;
        transform.LookAt(TargetTransform().position);
        m_Weapon.transform.LookAt(TargetTransform().position);
        m_LocationToMoveTo = TargetTransform().position;
    }

    public override void AttackLogic()
    {
        StopMoving();
        engagingTarget = true;
        if (m_Weapon)
        {
            Quaternion lookRotation = Quaternion.LookRotation(TargetTransform().position - m_BulletSpawningPosition.position);
            transform.LookAt(TargetTransform().position);
            m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);
            m_LocationToMoveTo = TargetTransform().position;
            m_Weapon.GetComponent<GunLogic>().Fire();
        }
    }

    public override void RunAndGunLogic()
    {
        ApplyDirection(m_LocationToMoveTo);
        m_Strafe = true;
        engagingTarget = true;
        if (m_Weapon)
        {
            Quaternion lookRotation = Quaternion.LookRotation(TargetTransform().position - m_BulletSpawningPosition.position);
            transform.LookAt(TargetTransform().position);
            m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);
            m_Weapon.GetComponent<GunLogic>().Fire();
        }
    }

    public override void RunAway()
    {
        Vector3 runAway = (TargetTransform().position - transform.position) * -2f;
        m_LocationToMoveTo = runAway;
        RunAndGunLogic();
    }

    public override void AlertLogic()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius * 20f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Enemy")
            {
                hitColliders[i].GetComponent<AIController>().alertOnTarget = true;
                hitColliders[i].GetComponent<AIController>().m_LocationToMoveTo = TargetTransform().position;
            }
            i++;
        }
    }
    public override void HideLogic()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius * 20f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Arcade Obstacle")
            {               
                var targetWeapon = GetTargetWeapon();
                Vector3 targetBulletSpawningPosition;
                if (targetWeapon)
                {
                    targetBulletSpawningPosition = targetWeapon.GetComponent<GunLogic>().m_BulletSpawnPoint.position;
                }
                else
                {
                    targetBulletSpawningPosition = TargetTransform().position;
                }
                float distance = Vector3.Distance(targetBulletSpawningPosition, hitColliders[i].transform.position);
                if (Physics.Raycast(targetBulletSpawningPosition, (targetBulletSpawningPosition - hitColliders[i].transform.position).normalized, out hit, distance))
                {
                    Debug.Log("hiding near " + hitColliders[i].name);
                    m_LocationToMoveTo = hitColliders[i].transform.position + (targetBulletSpawningPosition - hitColliders[i].transform.position).normalized * hitColliders[i].bounds.size.magnitude;
                    return;
                }
            }
            i++;
        }
    }

    void AttackTarget()
    {
        if (m_Target == Targets.player)
        {
            m_PlayerController.AddForce((m_MovementDirection + new Vector3(0, 2, 0)) * 20.0f); ;
        }
        else if (m_Target == Targets.friend)
        {
            m_FriendController.AddForce((m_MovementDirection + new Vector3(0, 2, 0)) * 20.0f); ;
        }
    }

    public GameObject GetTargetWeapon()
    {
        if (m_Target == Targets.player)
        {
            return m_PlayerController.GetWeapon();
        }
        else if (m_Target == Targets.friend)
        {
            return m_FriendController.GetWeapon();
        }
        else
        {
            return null;
        }
    }

    public float DistanceFromTarget()
    {
        if (m_Target == Targets.player)
        {
            return Vector3.Distance(m_PlayerTransform.position, transform.position);
        }
        else if (m_Target == Targets.friend)
        {
            return Vector3.Distance(m_FriendTransform.position, transform.position);
        }
        else
        {
            return 0f;
        }
    }

    string TargetTag()
    {
        if (m_Target == Targets.player)
        {
            return "Player";//m_PlayerController.tag;
        }
        else if (m_Target == Targets.friend)
        {
            return "Friend";//m_FriendController.tag;
        }
        else
        {
            return "";
        }
    }

    public Transform TargetTransform()
    {
        if (m_Target == Targets.player)
        {
            return m_PlayerTransform;
        }
        else if (m_Target == Targets.friend)
        {
            return m_FriendTransform;
        }
        else
        {
            return null;
        }
    }

    void ToggleTarget()
    {
        if(!m_FriendController)
        {
            m_Target = Targets.player;
            return;
        }
        if ( Vector3.Distance(m_PlayerTransform.position, transform.position) > Vector3.Distance(m_FriendTransform.position, transform.position))
        {
            m_Target = Targets.friend;
        }
        else
        {
            m_Target = Targets.player;
        }
    }

    bool IsTargetFiring()
    {
        if (m_Target == Targets.player)
        {
            return m_PlayerController.IsGunFiring();
        }
        else if (m_Target == Targets.friend)
        {
            return false;
        }
        return false;
    }



}
