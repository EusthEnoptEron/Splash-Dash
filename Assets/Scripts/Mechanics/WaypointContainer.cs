using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class WaypointContainer : MonoBehaviour
{
    public List<Waypoint> waypoints;

    void Awake()
    {
        waypoints = new List<Waypoint>(GetComponentsInChildren<Waypoint>());
    }

    public Waypoint GetNextWaypoint(Waypoint waypoint)
    {
        return IsLast(waypoint)
              ? waypoints[0]
              : waypoints[waypoint.id + 1];
    }


    public bool IsLast(Waypoint waypoint)
    {
        return waypoint.id == waypoints.Count - 1;
    }

    void Start()
    {
        //Waypoint.InitAll();
    }
}
