using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseAIController : MonoBehaviour
{

    public SpawnPoint m_SpawnPoint = null;

    public int instanceID;

    //The AI State obviously!
    public AIState m_State;

    //Time elapsed in state
    public float m_StateTime = 0f;

    // The character's running speed
    [SerializeField]
    protected float m_MovementSpeed = 4.0f;

    // The gravity strength
    [SerializeField]
    protected float m_Gravity = 60.0f;

    // The maximum speed the character can fall
    [SerializeField]
    protected float m_MaxFallSpeed = 20.0f;


    [SerializeField]
    protected float m_SightRange;

    public Waypoint m_CurrentWaypoint = null;

    [SerializeField]
    protected GameObject m_Weapon = null;

    //The navmesh agent
    [SerializeField]
    protected NavMeshAgent m_Agent;

    // The charactercontroller of the player
    protected CharacterController m_CharacterController;

    // The current movement direction in x & z.
    protected Vector3 m_MovementDirection = Vector3.zero;

    // The current vertical / falling speed
    protected float m_VerticalSpeed = 0.0f;

    // The current movement offset
    protected Vector3 m_CurrentMovementOffset = Vector3.zero;

    //The destination of the NavMeshAgent
    protected Vector3 m_Destination;

    // Whether the player is alive or not
    public bool m_IsAlive = true;

    protected Transform m_BulletSpawningPosition = null;

    public Vector3 m_LocationToMoveTo;

    //if true character will not rotate towards his movement direction
    protected bool m_Strafe = false;


    // The force added to the player (used for knockbacks)
    protected Vector3 m_Force = Vector3.zero;

    //If true the ai will not work so that another script can take over
    public bool levelScriptControl = false;

    //A target to look at
    public Vector3 m_TargetToLookAt = Vector3.zero;


    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Destination = transform.position;
        instanceID = GetInstanceID();
    }

    protected void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_Gravity * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeed);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeed);
    }

    protected void ApplyRotation()
    {
        // Rotate the character in movement direction
        if (m_MovementDirection != Vector3.zero)
        {
           
            RotateCharacter(m_MovementDirection);
            
        }
    }

    protected void ApplyDirection(Vector3 whereToMove)
    {
        m_Destination = whereToMove;
        ResumeMoving();
    }

    private void FixedUpdate()
    {
        if (!m_IsAlive)
        {
            return;
        }
        if (!levelScriptControl)
        {
            // Update jumping input and apply gravity
            ApplyGravity();

            // Rotate the character in movement direction
            ApplyRotation();

            ApplyMovement();
        }
    }

    protected void ApplyMovement()
    {
        m_Agent.destination = m_Destination;
        m_MovementDirection = m_Agent.desiredVelocity;
       // m_Agent.updatePosition = false;
       // m_Agent.updateRotation = false;
        m_MovementDirection.y = 0;
        m_MovementDirection.Normalize();

        // Calculate actual motion
        m_CurrentMovementOffset = (m_MovementDirection * m_MovementSpeed + m_Force + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;
        m_Force *= 0.95f;

    }


    //            AI ACTIONS            //


    public void Warp(Vector3 position)
    {
        m_Agent.Warp(position);
    }


    public virtual void Talk()
    {
        StopMoving();
    }

    public virtual void IddleLogic()
    {
        StopMoving();
    }

    public virtual void MovementLogic()
    {
        ApplyDirection(m_LocationToMoveTo);
    }

    public virtual void LookAroundLogic()
    {
        StopMoving();
        transform.Rotate(0f, 100f*Time.deltaTime, 0f);
        if(m_Weapon)
        {
            m_Weapon.transform.Rotate(0f, 100f * Time.deltaTime, 0f);
        }
    }

    public virtual void RunAway()
    {

    }

    public virtual void ChaseLogic()
    {

    }

    public virtual void HideLogic()
    {

    }

    public virtual void AlertLogic()
    {

    }

    public virtual void FollowPlayer()
    {

    }

    public virtual void MoveAroundLogic()
    {

    }

    public virtual void PatrolLogic()
    {
        if(!m_CurrentWaypoint)
        {
            m_CurrentWaypoint = FindObjectOfType<Waypoint>();
        }
        ApplyDirection(m_CurrentWaypoint.transform.position);
    }

    public virtual void AttackLogic()
    {

    }

    public virtual void RunAndGunLogic()
    {

    }

    public virtual void LookAtTarget()
    {

    }

    public void PickUpWeapon(Equipable w)
    {
        DropWeapon();
        w.PickUp(this);
        m_Weapon = w.gameObject;
        m_SightRange = m_Weapon.GetComponent<GunLogic>().m_Range;
        m_BulletSpawningPosition = m_Weapon.GetComponent<GunLogic>().m_BulletSpawnPoint;
    }

    public void PickUpWeapon()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_CharacterController.radius * 3.5f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Equipable")
            {
                Equipable equipable = hitColliders[i].GetComponent<Equipable>();
                if (equipable)
                {
                    PickUpWeapon(equipable);
                }
                return;
            }
            i++;
        }
    }

    public void DropWeapon()
    {
        if(m_Weapon)
        {
            m_Weapon.GetComponent<Equipable>().Drop();
            m_Weapon = null;
            m_SightRange = 5f;
        }
    }


    //          AI DECISIONS            //
    public virtual bool IsTargetActive()
    {
        return false;
    }

    public virtual bool SeeTarget()
    {
        return false;
    }

    public virtual bool HearTarget()
    {
        return false;
    }

    public virtual bool ReachedWaypoint()
    {
        if (Vector3.Distance(m_CurrentWaypoint.transform.position, transform.position) < 2f || CheckIfCountDownElapsed(m_State.TimeLimit))
        {
            m_CurrentWaypoint = m_CurrentWaypoint.nextWaypoint;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool ReachedDestination()
    {
        if (Vector3.Distance(m_LocationToMoveTo, transform.position) < 2f || CheckIfCountDownElapsed(m_State.TimeLimit))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool ShouldIHide()
    {
        Health h = GetComponent<Health>();
        if (h.m_TakingFire)
        {
            h.m_TakingFire = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool FindPlaceToHide()
    {
        return false;
    }

    public virtual bool DoINeedWeapon()
    {
        return !m_Weapon;
    }

    public virtual bool FindWeapon()
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
                    m_LocationToMoveTo = hitColliders[i].transform.position;
                    return true;
                }
            }
            i++;
        }
        return false;
    }

    public virtual bool CanAttackTarget()
    {
        return false;
    }

    public virtual bool TargetInRange()
    {
        return false;
    }

    public virtual bool TooCloseToPlayer()
    {
        return false;
    }

    public virtual bool TooFarFromPlayer()
    {
        return false;
    }

    public virtual bool IsSomethingInFrontOfMe()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.up * m_CharacterController.height / 4 + transform.forward, m_CharacterController.radius);
        if (hitColliders.Length != 0)
        {
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].tag == tag || hitColliders[i].tag == "Bullet")
                {
                    i++; //Comment this out to stuck the game in an infinite loop
                    continue;
                }

                if (hitColliders[i].tag != "Player")
                {
                    Debug.Log(hitColliders[i].name + " in front of me");
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    public virtual bool IsSomethingInFrontOfMyWeapon()
    {
        if(!m_Weapon)
        {
            return false;
        }
        Collider[] hitColliders = Physics.OverlapSphere(m_BulletSpawningPosition.position, m_CharacterController.radius);
        if (hitColliders.Length != 0)
        {
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].tag == tag || hitColliders[i].tag == "Bullet")
                {
                    i++; //Comment this out to stuck the game in an infinite loop
                    continue;
                }

                if (hitColliders[i].tag != "Player")
                {
                    Debug.Log(hitColliders[i].name + " in front of me");
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    public virtual bool CanCoverFire()
    {
        return false;
    }

    public virtual bool AmICovered()
    {
        return false;
    }

    public virtual bool AmIAlerted()
    {
        return false;
    }

    public virtual bool AmITalking()
    {
        return false;
    }

    protected float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    protected void RotateCharacter(Vector3 movementDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation && !m_Strafe)
        {
            transform.rotation = lookRotation;
        }
        if(m_Weapon)
        {
            //m_Weapon.transform.rotation = lookRotation;
            m_Weapon.GetComponent<Equipable>().UpdateRotation(lookRotation);  
        }
    }

    public bool IsMyGunBetter(GunLogic otherGun)
    {
        if(m_Weapon)
        {
            GunLogic myGun = m_Weapon.GetComponent<GunLogic>();            
            return (int)myGun.m_GunType >= (int)otherGun.m_GunType;
        }
        else
        {
            return false;
        }
    }


    public virtual void Die()
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
        m_StateTime += Time.deltaTime;
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
        else
        {
            Destroy(gameObject);
        }
    }

    public void Revive()
    {
        m_IsAlive = true;
        m_Agent.enabled = true;
        var collider = GetComponent<CapsuleCollider>();
        collider.enabled = true;
        GetComponent<Health>().m_Health = 100;
    }

    public void AddForce(Vector3 force)
    {
        m_Force += force;
    }

    public void StopMoving()
    {
        m_MovementDirection = Vector3.zero;
        m_CurrentMovementOffset = Vector3.zero;
        m_Destination = transform.position;
        m_Agent.isStopped = true;
    }

    public void ResumeMoving()
    {
        m_Agent.isStopped = false;
    }

    public void ChangeAIState(AIState state)
    {
        if (state != m_State)
        {
            m_State = state;
            m_StateTime = 0f;
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        m_StateTime += Time.deltaTime;
        return (m_StateTime >= duration);
    }

    public bool IsFiring()
    {
        if(!m_Weapon)
        {
            return false;
        }
        else
        {
            return m_Weapon.GetComponent<GunLogic>().IsGunFiring();
        }
    }

    private void OnDestroy()
    {
        if(m_SpawnPoint)
        {
            m_SpawnPoint.m_NumberOfActiveEnemies--;
            if(m_SpawnPoint.m_NumberOfActiveEnemies<0)
            {
                m_SpawnPoint.m_NumberOfActiveEnemies = 0;
            }
        }
    }


    
    public GameObject GetWeapon() { return m_Weapon; }
}
