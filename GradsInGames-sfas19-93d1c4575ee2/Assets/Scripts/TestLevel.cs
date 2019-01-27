using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestLevel : MonoBehaviour {

    [SerializeField]
    NavMeshSurface navMeshSurface;
	// Use this for initialization
	void Awake () {
        navMeshSurface.BuildNavMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
