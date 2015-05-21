using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(WaypointContainer))]
public class WaypointContainerEditor : Editor
{

    private static bool m_editMode = false;
    private static int m_count = 0;
    private GameObject m_container;


    void OnSceneGUI()
    {

        if (m_editMode)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.type == EventType.MouseDown)
            {

                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;


                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    int id = m_container.transform.childCount;

                    GameObject waypoint = Resources.Load<GameObject>("Prefabs/pref_Waypoint");
                    GameObject waypointInstance = Instantiate<GameObject>(waypoint);
                    waypointInstance.transform.position = hitInfo.point;
                    waypointInstance.name = "Waypoint" + id.ToString("00");
                    waypointInstance.transform.parent = m_container.transform;
                    Waypoint waypointScript = waypointInstance.GetComponent<Waypoint>();
                    waypointScript.id = id;


                    Selection.activeGameObject = m_container;
                    EditorUtility.SetDirty(waypointInstance);

                    m_count++;
                }

            }

            Event.current.Use();

        }

    }
    public override void OnInspectorGUI()
    {
        if (m_editMode)
        {
            if (GUILayout.Button("Disable Editing"))
            {
                m_editMode = false;
            }
        }
        else
        {
            if (GUILayout.Button("Enable Editing"))
            {
                m_editMode = true;

                // GET Current number of waypoints currently in window and start from there
                UnityEngine.Object[] waypoints = FindObjectsOfType(typeof(Waypoint));
                m_count = waypoints.Length;
                if (m_count < 0) m_count = 0;

                m_container = Selection.activeGameObject;

            }
        }

        if (GUILayout.Button("Reset"))
        {
            m_count = 0;
        }

    }


}