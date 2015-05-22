using UnityEngine;
using System.Collections;
using System;

//-----------------------------------------------------------------
// Waypoint
//-----------------------------------------------------------------

public class Waypoint : MonoBehaviour, IComparable
{

    public int id;

    public ArrayList targets;

    private Waypoint m_target;
    private int m_steps = 2;
    private float m_stepWidth = 5f;
    private float m_angle;
    private bool m_initilised = false;



    public int CompareTo(object obj)
    {
        Waypoint u = (Waypoint)obj;
        return this.id.CompareTo(u.id);
    }

    public void OnDrawGizmos()
    {


        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * 5f);

        if (!m_initilised) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, m_target.transform.position);


        /*
        for (int j = 0; j < targets.Count; j++)
        {
            Vector3 position = (Vector3)targets[j];
            Gizmos.color = Color.white;
            Gizmos.DrawCube(position, Vector3.one);
        }
        */


        //Gizmos.DrawLine (transform.position - range, range + transform.position);
    }
}