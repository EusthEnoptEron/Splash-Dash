using UnityEngine;
using System.Collections;

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
    private Color[,] colorMat = new Color[4096, 4096];

    private RenderTexture rPaintTexture;
    private int counter = 0;
    public const float SCALE_FACTOR = 1;


    public static PaintBrush Locate()
    {
        return GameObject.FindGameObjectWithTag("Paintbrush").GetComponent<PaintBrush>();
    }
	// Use this for initialization
    void Start () {
       
        rPaintTexture = new RenderTexture(4096, 4096, 8);
        rPaintTexture.enableRandomWrite = true;
        rPaintTexture.Create();
        computeShader.SetTexture(0, "Result", rPaintTexture);

        Shader.SetGlobalTexture("_PaintTexture", rPaintTexture);
        Shader.SetGlobalFloat("_PaintScale", SCALE_FACTOR);
	}
    
    void OnApplicationQuit()
    {
        Shader.SetGlobalFloat("_PaintScale", 0);
    }

    public void Paint(Vector3 worldPos, Color color, int width = 20, bool forward = true)
    {
        width = Mathf.Max(5, width);

        var pixelPos = WorldToPixelSpace(worldPos);

        int left = (int)(pixelPos.x) - width / 2;
        int top = (int)(pixelPos.y) - width / 2;

        //paintTexture.SetPixels(left, top, Mathf.RoundToInt(width / 2f), Mathf.RoundToInt(width / 2f), Enumerable.Repeat(color, width * width).ToArray() );

        //dirty = true;
        computeShader.SetVector("Color", color);
        computeShader.SetVector("Offset", new Vector2(left, top));
        computeShader.SetFloat("Radius", width / 2);
        computeShader.Dispatch(0, width / 5, width / 5, 1);

        for (int y = 0; y < width; y++)
        {
            int ry = top + y;
            if (ry >= 0 && ry < colorMat.GetLength(1))
            {
                for (int x = 0; x < width; x++)
                {
                    int rx = left + x;
                    if (rx >= 0 && rx < colorMat.GetLength(0))
                    {
                        colorMat[rx, ry] = color;
                    }
                }
            }
        }

        if(forward)
            networkView.RPC("PaintRPC", RPCMode.OthersBuffered, worldPos, color.r, color.g, color.b, color.a, width);
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
            (int)(Mathf.Clamp(worldPos.x * SCALE_FACTOR + 2048, 0, colorMat.GetLength(0))),
            (int)(Mathf.Clamp(worldPos.z * SCALE_FACTOR + 2048, 0, colorMat.GetLength(1)))
        );
    }

	// Update is called once per frame
	void Update () {
        //Debug.Log(counter);
        counter = 0;
	}

    protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
    }
}
