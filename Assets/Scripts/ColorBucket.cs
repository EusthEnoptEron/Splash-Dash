using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorBucket : MonoBehaviour {

    /// <summary>
    /// How many colors the bucket can hold.
    /// </summary>
    public int capacity = 5;

    private List<Color> m_Colors = new List<Color>();

    public Color Color
    {
        get
        {
            if (m_Colors.Count == 0) return new Color(0, 0, 0, 0);

            // Calculate average over all colors
            Color resultingColor = Color.clear;
            foreach (var color in m_Colors)
            {
                resultingColor.r += color.r / m_Colors.Count;
                resultingColor.g += color.g / m_Colors.Count;
                resultingColor.b += color.b / m_Colors.Count;
            }
            resultingColor.a = m_Colors.Count / capacity;

            return resultingColor;
        }
    }

    public IEnumerable<Color> Colors
    {
        get
        {
            return m_Colors;
        }
    }

    public bool Add(Color color)
    {
        m_Colors.Add(color);

        if (m_Colors.Count > capacity)
        {
            Overflow();
            return false;
        }
        return true;
    }

    private void Overflow()
    {
        m_Colors.Clear();
    }

    /// <summary>
    /// Turns the contents of the color bucket into ammo
    /// </summary>
    /// <returns></returns>
    public Ammo Materialize()
    {
        var ammo = new GameObject().AddComponent<Ammo>();
        return ammo;
    }

    public bool IsFull
    {
        get
        {
            return m_Colors.Count == capacity;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
