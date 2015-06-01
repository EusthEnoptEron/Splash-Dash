using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode]
public class WaypointContainer : MonoBehaviour
{
    [HideInInspector]
    public List<Waypoint> waypoints;
    private Vector3[] smoothedWaypoints;

    void Awake()
    {
        waypoints = new List<Waypoint>(GetComponentsInChildren<Waypoint>());
        smoothedWaypoints = waypoints.Select(wp => wp.transform.position).ToArray().Smooth(2);
    }

    public Waypoint GetNextWaypoint(Waypoint waypoint)
    {
        return IsLast(waypoint)
              ? waypoints[0]
              : waypoints[waypoint.id + 1];
    }

    public Vector3 GetDirection(Vector3 point)
    {
        int minIndex = 0;
        float minDistance, distance;

        minDistance = float.PositiveInfinity;

        for (int i = 0; i < smoothedWaypoints.Length; i++)
        {
            distance = Vector3.Distance(smoothedWaypoints[i], point);
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }

        // Make sure we have a distance.
        int nextIndex = minIndex;
        while (Vector3.Distance(smoothedWaypoints[nextIndex], smoothedWaypoints[minIndex]) < 0.001f)
        {
            nextIndex = (nextIndex + 1) % smoothedWaypoints.Length;
        }

        return (smoothedWaypoints[nextIndex] - smoothedWaypoints[minIndex]).normalized;

    }

    public bool IsLast(Waypoint waypoint)
    {
        return waypoint.id == waypoints.Count - 1;
    }

    void Start()
    {
        //Waypoint.InitAll();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < smoothedWaypoints.Length; i++)
        {
            int nextIndex = (i + 1) % smoothedWaypoints.Length;

            Gizmos.DrawLine(smoothedWaypoints[i], smoothedWaypoints[nextIndex]);
        }

    }
}
