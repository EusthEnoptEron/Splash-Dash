using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Chaikin {
    public delegate T Multiplier<T>(T point, float factor);
    public delegate T Summer<T>(T point1, T point2);

    public static T[] Smooth<T>(T[] points, Multiplier<T> predicate, Summer<T> summer, int iterations, bool loop)
    {
        List<T> currentPoints = new List<T>(points);
        if (loop)
        {
            currentPoints.Insert(0, points.Last());
            currentPoints.Add(points.First());
        }

        for (int i = 0; i < iterations; i++)
        {
            T[] nextPoints = new T[currentPoints.Count * 2 - 2];
            for (int j = 0; j < currentPoints.Count - 1; j++)
            {
                nextPoints[j * 2] = summer(predicate(currentPoints[j], 0.75f), predicate(currentPoints[j + 1], 0.25f));
                nextPoints[j * 2 + 1] = summer(predicate(currentPoints[j], 0.25f), predicate(currentPoints[j + 1], 0.75f));
            }

            currentPoints = nextPoints.ToList();
        }

        if (loop)
        {
            currentPoints.RemoveAt(0);
            currentPoints.RemoveAt(currentPoints.Count - 1);
        }

        return currentPoints.ToArray();
    }
}

public static class ChaikinExtensions
{
    public static Vector3[] Smooth(this Vector3[] vectors, int iterations, bool loop = true)
    {
        return Chaikin.Smooth(vectors,
                            (p, k) => p * k, (p1, p2) => p1 + p2,
                            iterations, loop);
    }

    public static Quaternion[] Smooth(this Quaternion[] qs, int iterations, bool loop = true)
    {
        return Chaikin.Smooth(qs,
                            (q, k) => Quaternion.Slerp(Quaternion.identity, q, k), (q1, q2) => q1 * q2,
                            iterations, loop);
    }
}