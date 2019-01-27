using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public int m_NumberOfWaves;
    public int m_NumberOfActiveEnemies = 0;
    public float m_TimeBetweenSpawn;
    public float m_TimeElapsed;
    public bool autoSpawn = false;
    public bool spawnAllObjectsAtOnce = false;
    public bool spawnTwoAtATime = false;
    public GameObject[] m_ObjectsToSpawn;
    int counter = 0;
    public Waypoint m_Waypoint;

    public bool orderedToSpawn = false;
    bool hasSpawned = false;

    public bool HasSpawned()
    {
        return hasSpawned;
    }

    public void DelayedSpawn(float time)
    {
        Invoke("Spawn", time);
    }

    public void Spawn()
    {
        if(m_NumberOfWaves <= 0)
        {
            return;
        }
        if (spawnAllObjectsAtOnce)
        {
            for (int i = 0; i < m_ObjectsToSpawn.Length; i++)
            {
                var obj = Instantiate(m_ObjectsToSpawn[i], transform.position, Quaternion.identity, transform);
                var ai = obj.GetComponent<AIController>();
                if (ai)
                {
                    ai.m_SpawnPoint = this;
                    if(m_Waypoint)
                    {
                        ai.m_CurrentWaypoint = m_Waypoint;
                    }
                    m_NumberOfActiveEnemies++;
                    hasSpawned = true;     
                }
                 
            }
        }
        else
        {
            if(counter>= m_ObjectsToSpawn.Length)
            {
                counter = 0;
            }
            var obj = Instantiate(m_ObjectsToSpawn[counter], transform.position, Quaternion.identity, transform);
            var ai = obj.GetComponent<AIController>();
            if (ai)
            {
                ai.m_SpawnPoint = this;
                if (m_Waypoint)
                {
                    ai.m_CurrentWaypoint = m_Waypoint;
                }
                m_NumberOfActiveEnemies++;
                hasSpawned = true;
            }
               
            counter++;
            if(spawnTwoAtATime)
            {
                if (counter >= m_ObjectsToSpawn.Length)
                {
                    counter = 0;
                }
                var obj2 = Instantiate(m_ObjectsToSpawn[counter], transform.position, Quaternion.identity, transform);
                var ai2 = obj2.GetComponent<AIController>();
                if (ai2)
                {
                    ai2.m_SpawnPoint = this;
                    if (m_Waypoint)
                    {
                        ai2.m_CurrentWaypoint = m_Waypoint;
                    }
                    m_NumberOfActiveEnemies++;
                    hasSpawned = true;
                }
                counter++;
            }
        }
        m_NumberOfWaves--;
        m_TimeElapsed = 0;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(autoSpawn)
        {
            if(m_TimeElapsed > m_TimeBetweenSpawn)
            {
                Spawn();
            }
            m_TimeElapsed += Time.deltaTime;
        }
	}
}
