using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VolColEntity : MonoBehaviour
{
    private VolColEntity[] childs = new VolColEntity[8];
    private BoxCollider bc;
    public bool isLeaf = false;
    private bool onceTriggered = false;


    void Start()
    {

        if ((!isLeaf) && childs[0] == null)
        {
            this.bc = this.gameObject.GetComponent<BoxCollider>();
            int i = 0;
            foreach (Transform f in this.transform)
            {
                if (f.gameObject.name == "child")
                {
                    childs[i++] = f.gameObject.GetComponent<VolColEntity>();
                }
            }
        }

        if (isLeaf && bc == null)
        {
            this.bc = this.gameObject.GetComponent<BoxCollider>();
        }
    }

    /*public void makeOffspring()
    {
        for (int i = 0; i < 8; i++)
        {
            this.childs[i].Init(this.gameObject,this.childs[i].GetComponent<BoxCollider>());
        }
    }*/

    public void InitAsLeafNode(GameObject cubie)
    {
        GameObject voxel = Instantiate(cubie, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;
        foreach (Renderer renderer in voxel.GetComponentsInChildren<Renderer>())
        {
            // renderer.enabled = true;
        }

        voxel.transform.localScale = this.gameObject.transform.lossyScale;
        voxel.transform.Translate(0, -0.5f * this.gameObject.transform.lossyScale.y, 0);
        voxel.name = "Voxel";

        voxel.transform.parent = this.transform;
     /*   Rigidbody r = this.gameObject.AddComponent<Rigidbody>();

        r.isKinematic = true;
        r.useGravity = false;*/


        for (int i = 0; i < 8; i++)
        {
            this.childs[i] = null;
        }
        this.isLeaf = true;

    }

    public void Init(GameObject parent, BoxCollider bc, int nX, int nY, int nZ)
    {
        int childNumber = 8;

        if (nX == 0)
        {
            childNumber /= 2;
        }
        if (nY == 0)
        {
            childNumber /= 2;
        }
        if (nZ == 0)
        {
            childNumber /= 2;
        }

        for (int i = 0; i < childNumber; i++)
        {
            GameObject child = new GameObject();
            child.name = "child";
            child.transform.parent = parent.transform;
            child.transform.position = parent.transform.position;
            child.transform.localEulerAngles = new Vector3(0, 0, 0); 
            child.layer = LayerMask.NameToLayer("colliderTarget");
            this.childs[i] = child.gameObject.AddComponent<VolColEntity>() as VolColEntity;
        }

        this.gameObject.layer = LayerMask.NameToLayer("colliderTarget");

        this.bc = bc;
        Vector3 scale = new Vector3(1, 1, 1);

        if (nX > 0)
        {
            scale.x /= 2;
        }

        if (nY > 0)
        {
            scale.y /= 2;
        }


        if (nZ > 0)
        {
            scale.z /= 2;
        }

        for (int i = 0; i < childNumber; i++)
        {

            this.childs[i].gameObject.transform.localScale = scale;
            if (nX != 0)
            {
                this.childs[i].gameObject.transform.Translate(-new Vector3((this.childs[i].gameObject.transform.lossyScale.x) / 2, 0, 0));
            }
            if (nY != 0)
            {
                this.childs[i].gameObject.transform.Translate(-new Vector3(0, (this.childs[i].gameObject.transform.lossyScale.y) / 2, 0));
            }
            if (nZ != 0)
            {
                this.childs[i].gameObject.transform.Translate(-new Vector3(0, 0, (this.childs[i].gameObject.transform.lossyScale.z) / 2));
            }

            BoxCollider bcC = this.childs[i].gameObject.AddComponent<BoxCollider>();
            this.childs[i].bc = bcC;
            bcC.isTrigger = true;
            //  bcC.size = this.transform.localScale;
        }

        scale.x *= this.gameObject.transform.lossyScale.x;
        scale.y *= this.gameObject.transform.lossyScale.y;
        scale.z *= this.gameObject.transform.lossyScale.z;


        int j = 1;
        if (nX != 0)
        {
            this.childs[j++].gameObject.transform.Translate(scale.x, 0, 0f);
        }

        if (nZ != 0)
        {
            if (nX != 0)
            {
                this.childs[j++].gameObject.transform.Translate(scale.x, 0, scale.z);
            }
            this.childs[j++].gameObject.transform.Translate(0, 0, scale.z);
        }


        if (nY != 0)
        {
            this.childs[j++].gameObject.transform.Translate(0, scale.y, 0f);
            if (nX != 0)
            {
                this.childs[j++].gameObject.transform.Translate(scale.x, scale.y, 0f);
            }
            if (nZ != 0)
            {
                if (nX != 0)
                {
                    this.childs[j++].gameObject.transform.Translate(scale.x, scale.y, scale.z);
                }
                this.childs[j++].gameObject.transform.Translate(0, scale.y, scale.z);
            }
        }

        if (nX > 0)
        {
            nX--;
        }

        if (nY > 0)
        {
            nY--;
        }

        if (nZ > 0)
        {
            nZ--;
        }

        if (nX + nY + nZ == 0)
        {
            for (int i = 0; i < childNumber; i++)
            {
                this.childs[i].gameObject.GetComponent<VolColEntity>().InitAsLeafNode(GameObject.Find("cubieThree"));
            }
        }
        else
        {
            for (int i = 0; i < childNumber; i++)
            {
                this.childs[i].Init(this.childs[i].gameObject, this.childs[i].bc, nX, nY, nZ);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void triggerNOpt(Collider projectile)
    {
        GameObject voxel = this.transform.Find("Voxel").gameObject;
        Vector3 dir = this.transform.position - projectile.transform.position;
       // paint(voxel, new Color(dir.x,dir.y,dir.z));

        dir.Normalize();

        enableFace(-dir, voxel);
        

    }

    private void enableFace(Vector3 dir, GameObject voxel)
    {
        if (voxel.transform.parent.GetComponent<VolColEntity>().onceTriggered)
        {
            return;
        }
        GameObject g;
        if ((dir - new Vector3(0, 1, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("top").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = !g.GetComponent<Renderer>().enabled;
            }
            return;
        }

        if ((dir - new Vector3(0, 0, 1)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("right").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = !g.GetComponent<Renderer>().enabled;
            }
            return;
        }

        if ((dir - new Vector3(1, 0, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("back").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = !g.GetComponent<Renderer>().enabled;
            }
            return;
        }

        if ((dir - new Vector3(0, -1, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("bot").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = !g.GetComponent<Renderer>().enabled;
            }
            return;
        }

        if ((dir - new Vector3(0, 0, -1)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("left").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = !g.GetComponent<Renderer>().enabled;
            }
            return;
        }

        if ((dir - new Vector3(-1, 0, 0)).magnitude < 0.01f)
        {
            g = voxel.transform.Find("front").gameObject;
            if (g != null)
            {
                g.GetComponent<Renderer>().enabled = !g.GetComponent<Renderer>().enabled;
            }
            return;
        }

    }

    private void paint(GameObject p, Color c)
    {
        foreach (Renderer r in p.GetComponentsInChildren<Renderer>())
        {
            r.material.color = c;
        }
    }

    public void trigger()
    {
        if (this.transform.Find("Voxel") != null)
        {
            GameObject voxel = this.transform.Find("Voxel").gameObject;
            if (!this.onceTriggered)
            {
                GameObject neighOpt = new GameObject();
                neighOpt.name = "colliderNOpt";
                neighOpt.tag = "colliderNOpt";
                neighOpt.layer = LayerMask.NameToLayer("colliderNOpt");
              
                neighOpt.transform.position = voxel.transform.position;
                neighOpt.transform.rotation = voxel.transform.rotation;
                neighOpt.transform.localScale = voxel.transform.lossyScale;
                neighOpt.transform.parent = voxel.transform.parent;
                neighOpt.transform.Translate(0, +0.5f * voxel.transform.lossyScale.y, 0);

                SphereCollider sc = neighOpt.AddComponent<SphereCollider>();
                sc.isTrigger = true;
                sc.radius = 0.6f;

                foreach (Renderer r in voxel.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }

                foreach (BoxCollider bc in voxel.GetComponentsInChildren<BoxCollider>())
                {
                    bc.enabled = false;
                }
            }
            this.onceTriggered = true;


        }
    }

    public void traverseTree(Collider projectile)
    {

        if (this.isLeaf)
        {

            if (projectile.gameObject.layer == LayerMask.NameToLayer("colliderNOpt"))
            {
                if (VolCol.customTriggerFunctionEnabled)
                {
                    VolCol.customTriggerFunction(this, projectile);
                }
                else
                {
                    this.triggerNOpt(projectile);
                }
            }
            else
            {
                this.trigger();
            }
        }
        else
        {
            foreach (VolColEntity child in this.childs)
            {
                if (child != null)
                {
                    if (child.bc.bounds.Intersects(projectile.bounds))
                    {
                        child.traverseTree(projectile);
                    }
                }
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        //Ignore for now
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision VolColEntity");
    }

}
