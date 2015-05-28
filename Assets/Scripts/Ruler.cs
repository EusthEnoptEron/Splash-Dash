using UnityEngine;
using System.Collections;

public static class Ruler  {


    /// <summary>
    /// Measures the size of the game field.
    /// </summary>
    /// <returns></returns>
    public static Vector2 Measure()
    {
        var road = GameObject.FindGameObjectWithTag("Road");

        var bounds = road.GetComponent<MeshRenderer>().bounds;

        return new Vector2(
            Mathf.Max( Mathf.Abs(bounds.min.x), Mathf.Abs(bounds.max.x)) * 2 + 100,
            Mathf.Max( Mathf.Abs(bounds.min.z), Mathf.Abs(bounds.max.z)) * 2 + 100
        );
    }
}
