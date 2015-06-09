﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class PaintBrush : NetworkBehaviour {
    struct Point
    {
        public int x; public int y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public ComputeShader computeShader;
    private Color[,] colorMat = new Color[TEXTURE_SIZE, TEXTURE_SIZE];

    private RenderTexture rPaintTexture;
    private Texture2D _paintTexture;

    private int counter = 0;
    private bool _dirty = false;
    private const int TEXTURE_SIZE = 4096;

    [SerializeField]
    private bool _adaptiveScale = true;

    public static float SCALE_FACTOR = 1;

    public bool useComputeShader = true;


    public static PaintBrush Locate()
    {
        var paintbrushObj = GameObject.FindGameObjectWithTag("Paintbrush");
        if (paintbrushObj == null)
        {
            paintbrushObj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/pref_PaintBrush"));
        }
        return paintbrushObj.GetComponent<PaintBrush>();
    }


    protected override void Awake()
    {
        base.Awake();
        networkView.stateSynchronization = NetworkStateSynchronization.Off;

        if (useComputeShader)
        {
            rPaintTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 8);
            rPaintTexture.enableRandomWrite = true;
            rPaintTexture.Create();
            computeShader.SetTexture(0, "Result", rPaintTexture);
        }
        else
        {
            _paintTexture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, false);
            _paintTexture.SetPixels(0, 0, TEXTURE_SIZE, TEXTURE_SIZE, Enumerable.Repeat(Color.clear, TEXTURE_SIZE * TEXTURE_SIZE).ToArray());
            _dirty = true;
        }

        var mapSize = Ruler.Measure();
        float maxLength = Mathf.Max(mapSize.x, mapSize.y);
        
        if(_adaptiveScale)
            SCALE_FACTOR = TEXTURE_SIZE / maxLength;

        Debug.Log("Scale factor: " + SCALE_FACTOR);

        if(useComputeShader)
            Shader.SetGlobalTexture("_PaintTexture", rPaintTexture);
        else
            Shader.SetGlobalTexture("_PaintTexture", _paintTexture);

        Shader.SetGlobalFloat("_PaintScale", SCALE_FACTOR);
        Shader.SetGlobalFloat("_GlossinessPaint", 0.7f);
        Shader.SetGlobalFloat("_MetallicPaint", 0.7f);
        Shader.SetGlobalFloat("_TextureWidth", TEXTURE_SIZE);

    }

    void Start()
    {
        StartCoroutine(UpdateTexture());
    }
    void OnApplicationQuit()
    {
        Shader.SetGlobalFloat("_PaintScale", 0);
    }

    public void Paint(Vector3 worldPos, Color color, int width = 20, bool forward = true)
    {
        //if (_dirty) return;

        width = Mathf.Max(5, width);

        var pixelPos = WorldToPixelSpace(worldPos);

        int left = (int)(pixelPos.x) - width / 2;
        int top = (int)(pixelPos.y) - width / 2;

        //dirty = true;
        float radius = width / 2f;

        if (useComputeShader)
        {
            computeShader.SetVector("Color", color);
            computeShader.SetVector("Offset", new Vector2(left, top));
            computeShader.SetFloat("Radius", radius);
            computeShader.Dispatch(0, Mathf.CeilToInt(width / 5f), Mathf.CeilToInt(width / 5f), 1);
        }

        width -= Mathf.Max(0, (left + width) - (TEXTURE_SIZE - 1));
        width -= Mathf.Max(0, (top + width) - (TEXTURE_SIZE - 1));

        var vRadius = new Vector2(radius, radius);
        for (int y = 0; y < width; y++)
        {
            int ry = top + y;
            if (ry >= 0 && ry < colorMat.GetLength(1))
            {
                for (int x = 0; x < width; x++)
                {
                    var position = new Vector2(x, y);

                    int rx = left + x;
                    if (rx >= 0 && rx < colorMat.GetLength(0))
                    {
                        // Only draw within a circle. Same calculation as in the compute shader.
                        if ((position - vRadius).magnitude <= radius)
                        {
                            colorMat[rx, ry] = color;
                            if (!useComputeShader)
                            {
                                _paintTexture.SetPixel(rx, ry, color);
                            }
                        }
                    }
                }
            }
        }
        //_paintTexture.SetPixels(left, top, width, width, Enumerable.Repeat(color, width * width).ToArray());
        _dirty = true;

        if (forward && NetworkController.IsConnected)
        {
            networkView.RPC("PaintRPC", RPCMode.OthersBuffered, worldPos, color.r, color.g, color.b, color.a, width);
        }

    }

    [RPC]
    private void PaintRPC(Vector3 worldPos, float r, float g, float b, float a, int width)
    {
        Paint(worldPos, new Color(r, g, b, a), width, false);
    }

    public Color GetColor(Vector3 worldPos)
    {
        var pixelPos = WorldToPixelSpace(worldPos);
        return colorMat[pixelPos.x, pixelPos.y];
    }

    private Point WorldToPixelSpace(Vector3 worldPos)
    {
        return new Point(
            (int)(Mathf.Clamp(worldPos.x * SCALE_FACTOR + TEXTURE_SIZE / 2, 0, colorMat.GetLength(0) - 1)),
            (int)(Mathf.Clamp(worldPos.z * SCALE_FACTOR + TEXTURE_SIZE / 2, 0, colorMat.GetLength(1) - 1))
        );
    }

	// Update is called once per frame
	void Update () {
        //Debug.Log(counter);
        //counter = 0;

        //if (_dirty)
        //{
        //    _paintTexture.Apply(false, false);
        //    _dirty = false;
        //}
	}


    IEnumerator UpdateTexture()
    {
        if (useComputeShader) yield break;

        while (true)
        {
            if (_dirty)
            {
                _paintTexture.Apply(false, false);
                _dirty = false;
                //yield return new WaitForSeconds(0.5f);

            }
            yield return null;
        }
    }
    protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
    }
}
