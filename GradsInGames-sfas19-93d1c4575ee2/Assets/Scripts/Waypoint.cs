using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public Waypoint nextWaypoint = null;
    public Waypoint previousWaypoint = null;

    private void Start()
    {
        if (!nextWaypoint)
        {
            nextWaypoint = this;
        }
        if (!previousWaypoint)
        {
            previousWaypoint = this;
        }
    }

}
