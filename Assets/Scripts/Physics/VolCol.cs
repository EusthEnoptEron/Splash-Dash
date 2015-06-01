using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VolCol : MonoBehaviour
{

    public int numberOfSubdividesX = 1;
    public int numberOfSubdividesY = 1;
    public int numberOfSubdividesZ = 1;


    private static int numberOfSegmentsXOld;
    private static int numberOfSegmentsYOld;
    private static int numberOfSegmentsZOld;

    private VolColEntity headTree;
    private bool recreate = false;

    public static bool customTriggerFunctionEnabled = false;
    public delegate void customTriggerFunc(VolColEntity e, Collider c);
    public static customTriggerFunc customTriggerFunction;

    System.Collections.Generic.List<Collider> watchedColliders = new System.Collections.Generic.List<Collider>();



    // Use this for initialization
    void Start()
    {
        GameObject volumericCollisionHandled = GameObject.Find("volumericCollisionHandled");
        SphereCollider sc;
        BoxCollider bc;

        if (volumericCollisionHandled == null)
        {
            volumericCollisionHandled = new GameObject();
            volumericCollisionHandled.name = "volumericCollisionHandled";
            sc = volumericCollisionHandled.AddComponent<SphereCollider>();
            volumericCollisionHandled.transform.parent = this.transform;
            volumericCollisionHandled.transform.position = this.transform.position;

            bc = this.gameObject.AddComponent<BoxCollider>();
        }
        else
        {
            sc = volumericCollisionHandled.GetComponent<SphereCollider>();
            bc = GetComponent<BoxCollider>();
        }

        sc.radius = 1;
        sc.isTrigger = true;
        volumericCollisionHandled.layer = LayerMask.NameToLayer("colliderGuardian");
        //  bc.size = this.transform.localScale;

        Load();
        if (numberOfSubdividesX != numberOfSegmentsXOld || numberOfSubdividesY != numberOfSegmentsYOld || numberOfSubdividesZ != numberOfSegmentsZOld)
        {
            this.recreate = true;
            numberOfSegmentsXOld = numberOfSubdividesX;
            numberOfSegmentsYOld = numberOfSubdividesY;
            numberOfSegmentsZOld = numberOfSubdividesZ;
            Save();
        }

        if (this.recreate || (GameObject.Find("childHead") == null))
        {
            Debug.Log("recreate");
            DestroyImmediate(this.headTree);
            DestroyImmediate(GameObject.Find("childHead"));

            GameObject child = new GameObject();
            child.name = "childHead";
            child.transform.position = this.transform.position;
            child.transform.parent = this.transform;
            child.transform.localEulerAngles = new Vector3(0, 0, 0);

            child.transform.localScale = new Vector3(1, 1, 1);
            this.transform.localScale = new Vector3(1f / (Mathf.Pow(2,numberOfSubdividesY)), this.transform.localScale.y, this.transform.localScale.z);

            this.headTree = child.AddComponent<VolColEntity>() as VolColEntity;
            BoxCollider bc2 = child.AddComponent<BoxCollider>();
            bc2.size = bc.size;
            bc2.isTrigger = true;
            headTree.transform.parent = this.transform;
            headTree.Init(child, bc2, this.numberOfSubdividesX, this.numberOfSubdividesY, this.numberOfSubdividesZ);

            this.recreate = false;
        }
        else
        {
            this.headTree = GameObject.Find("childHead").GetComponent<VolColEntity>();
        }

        if (Application.isPlaying)
        {
            customTriggerFunctionEnabled = true;
            customTriggerFunction = this.customTriggerOuterShellEnable;

            createOuterShellCollider();
        }

    }

    public static void Load()
    {
        numberOfSegmentsXOld = PlayerPrefs.GetInt("numberOfSegmentsXOld", 0);
        numberOfSegmentsYOld = PlayerPrefs.GetInt("numberOfSegmentsYOld", 0);
        numberOfSegmentsZOld = PlayerPrefs.GetInt("numberOfSegmentsZOld", 0);

    }

    public static void Save()
    {
        PlayerPrefs.SetInt("numberOfSegmentsXOld", numberOfSegmentsXOld);
        PlayerPrefs.SetInt("numberOfSegmentsYOld", numberOfSegmentsYOld);
        PlayerPrefs.SetInt("numberOfSegmentsZOld", numberOfSegmentsZOld);
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        foreach (Collider projectile in this.watchedColliders)
        {
            if (projectile.bounds.Intersects(this.GetComponent<BoxCollider>().bounds))
            {
                this.headTree.traverseTree(projectile);
            }
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("colliderNOpt"))
        {
            if (g.GetComponent<Collider>() != null)
            {
                this.headTree.traverseTree(g.GetComponent<Collider>());
                DestroyImmediate(g);
            }
            else
            {
                if (g.transform.childCount == 0)
                {
                    DestroyImmediate(g);
                }
            }
        }
        customTriggerFunctionEnabled = false;

    }

    void OnTriggerEnter(Collider other)
    {
        this.watchedColliders.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        this.watchedColliders.Remove(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision VolCol");
    }

    private void createOuterShellCollider()
    {
       
        if (GameObject.Find("OuterShells") == null)
        {
            GameObject OuterShells = new GameObject();

            OuterShells.name = "OuterShells";
            OuterShells.tag = "colliderNOpt";
            OuterShells.layer = LayerMask.NameToLayer("colliderNOpt");
            OuterShells.transform.parent = this.transform.parent;
            OuterShells.transform.position = this.transform.position;
            OuterShells.transform.localEulerAngles = new Vector3(0, 0, 0);
            OuterShells.transform.localScale = new Vector3(1,1,1);



            float t = 0.5f;
            createShellGameObj(new Vector3(0.0000000001f, 1,1), new Vector3(-0.499f * this.transform.lossyScale.x, (-0.5f + t) * this.transform.lossyScale.y, 0), 0, OuterShells);
            createShellGameObj(new Vector3(0.0000000001f,1, 1), new Vector3(0.499f * this.transform.lossyScale.x, (-0.5f + t) * this.transform.lossyScale.y, 0), 1, OuterShells);

            createShellGameObj(new Vector3(1, 1, 0.0000000001f), new Vector3(0, (-0.5f + t) * this.transform.lossyScale.y, -0.490f * this.transform.lossyScale.z), 2, OuterShells);
            createShellGameObj(new Vector3(1, 1, 0.0000000001f), new Vector3(0, (-0.5f + t) * this.transform.lossyScale.y, 0.499f * this.transform.lossyScale.z), 3, OuterShells);

            createShellGameObj(new Vector3(1, 0.0000000001f, 1), new Vector3(0, (-0.001f + t) * this.transform.lossyScale.y, 0), 4, OuterShells);
            createShellGameObj(new Vector3(1, 0.0000000001f, 1), new Vector3(0, (-0.999f + t) * this.transform.lossyScale.y, 0), 5, OuterShells);
               
        }
    }

    private void createShellGameObj(Vector3 scale, Vector3 translate,int i, GameObject parent)
    {
        GameObject OuterShellCollider = new GameObject();
        OuterShellCollider.name = "OuterShellCollider"+i;
        OuterShellCollider.tag = "colliderNOpt";
        OuterShellCollider.layer = LayerMask.NameToLayer("colliderNOpt");
        OuterShellCollider.transform.parent =parent.transform;
        
        OuterShellCollider.transform.localEulerAngles = new Vector3(0, 0, 0);
        OuterShellCollider.transform.localPosition = new Vector3(0,0,0);
        OuterShellCollider.transform.localScale = scale;
        OuterShellCollider.transform.Translate(translate);
        BoxCollider bc = OuterShellCollider.AddComponent<BoxCollider>();
    }

    public void customTriggerOuterShellEnable(VolColEntity e, Collider c)
    {
        Quaternion q = c.transform.parent.rotation;
        q = Quaternion.Inverse(q);
        Vector3 dir = q*(c.transform.position - this.transform.position);
       /* Debug.Log(c.transform.position.x + " " + this.transform.FindChild("childHead").position.x);
        Debug.Log(c.transform.position.y + " " + this.transform.FindChild("childHead").position.y);
        Debug.Log(c.transform.position.z+ " " + this.transform.FindChild("childHead").position.z);*/

        dir.Normalize();
        enableFace(dir, e.transform.Find("Voxel").gameObject);
    }

    private void enableFace(Vector3 dir, GameObject voxel)
    {

        GameObject g;
        if ((dir - new Vector3(0, 1, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("top").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = true;
            }
            return;
        }

        if ((dir - new Vector3(0, 0, 1)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("right").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = true;
            }
            return;
        }

        if ((dir - new Vector3(1, 0, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("back").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = true;
            }
            return;
        }

        if ((dir - new Vector3(0, -1, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("bot").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = true;
            }
            return;
        }

        if ((dir - new Vector3(0, 0, -1)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("left").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = true;
            }
            return;
        }

        if ((dir - new Vector3(-1, 0, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("front").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = true;
            }
            return;
        }

    }
}
