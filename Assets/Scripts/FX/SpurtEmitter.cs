using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SpurtEmitter : NetworkBehaviour
{
    class SpurtPosition
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Radius;
        public bool IsDead;
        public bool IsPainted;
        public Color Color;
        public float V;

        private static GameObject decalPrefab = Resources.Load<GameObject>("DecalPaint");
        public SpurtPosition(Vector3 startPosition, Vector3 velocity, float radius, Color color)
        {
            // TODO: Complete member initialization
            Position = startPosition;
            Velocity = velocity;
            Radius = radius;
            Color = color;
        }


        public void Step()
        {
            if (!IsDead)
            {
                var prevPos = Position;
                Velocity += Physics.gravity * Time.deltaTime;
                Position += Velocity * Time.deltaTime;
                var diff = Position - prevPos;

                Ray ray = new Ray(prevPos, diff.normalized);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, diff.magnitude, LayerMask.GetMask("Colorable")))
                {
                    Position = hit.point;
                    Velocity = -hit.normal;
                    //var decal = GameObject.Instantiate<GameObject>(decalPrefab);
                    //decal.transform.position = hit.point + hit.normal * 0.01f;
                    //decal.transform.rotation = Quaternion.LookRotation(-hit.normal);
                    //decal.transform.localScale = Vector3.one * Radius * 4;

                    // KILL
                    IsDead = true;
                    

                }
            }

        }
    }

    public int count = 100;
    public float startVelocity = 10;
    public int sides = 8;
    public float radius = 0.1f;
    public float shootDelay = 0.4f;

    private float _shooting = 0;

    private float prevRadius = 0.1f;
    public float radiusGrowthSpeed = 1;
    public Transform gunMesh;
    private Quaternion gunStartOrientation;

    private Color shootingColor = Color.clear;
    
    private MeshRenderer renderer;
    private MeshFilter meshFilter;
    private new Rigidbody rigidbody;
    private PaintBrush paintBrush;


    //private Mesh mesh;
    private LinkedList<SpurtPosition> positions = new LinkedList<SpurtPosition>();
    private int uvCounter;
    private PaintTank[] tanks;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        rigidbody = GetComponentInParent<Rigidbody>();

        gunStartOrientation = gunMesh.rotation;

        //mesh = meshFilter.mesh;
        
        for (int i = 0; i < count; i++)
        {
            positions.AddFirst((SpurtPosition)null);
        }

        paintBrush = GameObject.FindGameObjectWithTag("Paintbrush").GetComponent<PaintBrush>();
        tanks = GetComponentsInChildren<PaintTank>();
    }


    void UpdatePositions()
    {
        LinkedListNode<SpurtPosition> position = positions.First;
        while (position != null)
        {
            if (position.Value != null)
            {
                if (position.Value.IsDead)
                {
                    if (!position.Value.IsPainted && !IsRemoteControlled)
                    {
                        paintBrush.Paint(position.Value.Position, position.Value.Color, (int)(position.Value.Radius * 2 * PaintBrush.SCALE_FACTOR ));

                        if (position.Previous != null && position.Previous.Value != null && position.Previous.Value.IsDead)
                            PaintToPosition(position.Value, position.Previous.Value);

                        if (position.Next != null && position.Next.Value != null && position.Next.Value.IsDead)
                            PaintToPosition(position.Value, position.Next.Value);

                        position.Value.IsPainted = true;
                    }
                }
                else
                {
                    position.Value.Step();
                    position.Value.Radius += radiusGrowthSpeed * Time.deltaTime;
                }
                
            }
            position = position.Next;
        }
    }

    void PaintToPosition(SpurtPosition from, SpurtPosition to)
    {
        Vector3 pos = from.Position;
        float distance = Vector3.Distance(from.Position, to.Position);

        while (pos != to.Position)
        {
            pos = Vector3.MoveTowards(pos, to.Position, from.Radius);
            float remainingDistance = Vector3.Distance(pos, to.Position);

            paintBrush.Paint(pos, Color.Lerp(to.Color, from.Color, remainingDistance / distance), (int)(from.Radius * 2 * PaintBrush.SCALE_FACTOR));
        }
    }

    public Vector3 Inertia
    {
        get
        {
            //return Vector3.Dot(rigidbody ? rigidbody.velocity : Vector3.zero, transform.forward) * transform.forward;
            return rigidbody ? rigidbody.velocity * 1.3f : Vector3.zero;
        }
    }

    private List<PaintTank> startTanks = new List<PaintTank>();

    // Update is called once per frame
    void LateUpdate()
    {
   
        prevRadius = Mathf.Clamp(prevRadius * Random.Range(0.9f, 1.2f), radius / 4 * 3, radius / 4 * 5);
        prevRadius = radius;
        positions.RemoveLast();

        UpdatePositions();

        var validTanks = tanks.Where(tank => !tank.IsEmpty && tank.IsShootable);
        var minimalTanks = validTanks.Union(startTanks).Distinct().Where(tank => !tank.IsEmpty && (tank.IsShootable || (Time.time - tank.ReleaseTime) < shootDelay * 2 ) );

        _shooting = validTanks.Count() > 0 
            ? Mathf.MoveTowards(_shooting, 1, Time.deltaTime / (shootDelay * 2) ) 
            : Mathf.MoveTowards(_shooting, 0, Time.deltaTime / (shootDelay * 2) );

        if (minimalTanks.Count() == 0) _shooting = 0;

        if ((!IsRemoteControlled && _shooting >= 0.5f) || (IsRemoteControlled && shootingColor != Color.clear))
        {
            if (!IsRemoteControlled)
            {
                int tankCount = minimalTanks.Count();

                shootingColor = new Color(0,0,0,1);
                foreach (var tank in minimalTanks)
                {
                    tank.Consume(Time.deltaTime);
                    shootingColor.r += tank.color.r;
                    shootingColor.g += tank.color.g;
                    shootingColor.b += tank.color.b;
                }
            }
            startTanks = minimalTanks.ToList();

            positions.AddFirst(new SpurtPosition(transform.position, Inertia + transform.forward * startVelocity, prevRadius, shootingColor/* new HSBColor(Time.time % 1, 1, 1).ToColor()*/ )
            {
                V = (uvCounter++ % count - 1f) / count
            });
        }
        else
        {
            startTanks.Clear();
            positions.AddFirst((SpurtPosition)null);
            shootingColor = Color.clear;
        }

        //gunMesh.rotation = gunStartOrientation *  Quaternion.Inverse(gunStartOrientation) * transform.rotation;
        gunMesh.rotation = transform.rotation * Quaternion.Inverse(Quaternion.LookRotation(-Vector3.up, Vector3.forward));
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        var mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();

        bool previousHadValues = false;

        // Update positions
        int i = 0;
        foreach (var position in positions)
        {
            if (position != null && !position.IsDead)
            {
                // Set position
                var corners = new Vector3[sides];
                var fwd = position.Velocity.normalized;
                if (fwd.sqrMagnitude == 0) fwd = Vector3.down;

                var up = Vector3.ProjectOnPlane(Vector3.up, fwd).normalized;// Vector3.Cross(fwd, right).normalized;
                var right = Vector3.Cross(up, fwd).normalized;
                //var right = new Vector3(fwd.z, fwd.y, fwd.x);
                int startIndex = vertices.Count;

                for (int y = 0; y < sides; y++)
                {
                    float rate = y / (float)sides;
                    corners[y] = transform.InverseTransformPoint(
                                position.Position
                                + up * Mathf.Sin(rate * Mathf.PI * 2) * position.Radius
                                + right * Mathf.Cos(rate * Mathf.PI * 2) * position.Radius
                    );
                }

                if (previousHadValues)
                {
                    vertices.AddRange(corners);

                    // Connect with previous
                    for (int y = 0; y < sides; y++)
                    {
                        bool isLast = y == sides - 1;
                        int upperRight = startIndex + y;
                        int upperLeft = startIndex + y - sides;
                        int lowerRight = isLast ? (startIndex) : (startIndex + y + 1);
                        int lowerLeft = isLast ? (startIndex - sides) : (startIndex + y + 1 - sides);
                        normals.Add(Vector3.Cross(vertices[upperLeft] - vertices[upperRight],
                                                    vertices[lowerRight] - vertices[upperRight]).normalized);

                        colors.Add(position.Color);
                        uvs.Add(new Vector2(y / (sides - 1f), position.V));

                        tris.AddRange(new int[]{ 
                            upperRight, upperLeft, lowerRight,
                            upperLeft, lowerLeft, lowerRight,
                            //upperRight, lowerRight, upperLeft,
                            //upperLeft, lowerRight, lowerLeft
                        });

                    }
                }
                else
                {
                    var center = transform.InverseTransformPoint(position.Position);
                    int centerIndex = startIndex;

                    vertices.Add(center);
                    vertices.AddRange(corners);
                    startIndex++;

                    for (int y = 0; y < sides; y++)
                    {
                        bool isLast = y == sides - 1;
                        int nextIndex = isLast ? startIndex : startIndex + y + 1;
                        int current = startIndex + y;
                        tris.AddRange(new int[] {
                            current, centerIndex, nextIndex
                        });

                        normals.Add(-fwd);
                        colors.Add(position.Color);
                        uvs.Add(new Vector2(y / (sides - 1f), position.V));

                    }
                    normals.Add(-fwd);
                    colors.Add(position.Color);
                    uvs.Add(new Vector2(0.5f, position.V));


                }

                previousHadValues = true;
            }
            else
            {
                previousHadValues = false;
                //lineRenderer.SetPosition(i++, position.Position);
            }
            i++;
        }

        //Debug.LogFormat("{0} {1}", vertices.Count, normals.Count);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.colors = colors.ToArray();
        mesh.uv = uvs.ToArray();
        //mesh.normals = normals.ToArray();

        mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        // Let's abuse a quaternion for a color container
        Quaternion color = Quaternion.identity;
        Quaternion rotation = transform.localRotation;
        float r, g, b, a;
        r = g = b = a = 0;


        if (stream.isWriting)
        {
            // We're the lead
            color = new Quaternion(shootingColor.r, shootingColor.g, shootingColor.b, shootingColor.a);
            r = shootingColor.r;
            g = shootingColor.g;
            b = shootingColor.b;
            a = shootingColor.a;

            stream.Serialize(ref r);
            stream.Serialize(ref g);
            stream.Serialize(ref b);
            stream.Serialize(ref a);
            stream.Serialize(ref rotation);
        }
        else
        {
            // We're the copy-cat
            stream.Serialize(ref r);
            stream.Serialize(ref g);
            stream.Serialize(ref b);
            stream.Serialize(ref a);
            stream.Serialize(ref rotation);

            shootingColor = new Color(
                r < 0.1f ? 0 : 1,
                g < 0.1f ? 0 : 1,
                b < 0.1f ? 0 : 1,
                a < 0.1f ? 0 : 1
            );
            transform.localRotation = rotation;
        }
    }
}
