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

    void Start()
    {
        //Waypoint.InitAll();
    }
}
