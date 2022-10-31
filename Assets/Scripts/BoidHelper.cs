using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidHelper {

    const int numViewDirections = 180;
    public static readonly Vector2[] directions1;
    public static readonly Vector2[] directions2;
    public static readonly int[] angles;

    static BoidHelper () {
        directions1 = new Vector2[numViewDirections];
        directions2 = new Vector2[numViewDirections];
        angles = new int[180];
        float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 1; i < numViewDirections; i++) {
            float t = (float) i / numViewDirections;
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            //directions[i-1] = new Vector2 (x, y);
        }
        for (int i = 0; i < angles.Length/2; i++)
        {
            //directions1[i] = DirectionFromAngle(i);
            angles[2*i+1] = 2*i+1;
        }
        for (int i = 0; i < angles.Length/2; i++)
        {
           // directions2[i] = DirectionFromAngle(-i);
            angles[2*i] = -2*i-1;
        }
    }

    private static Vector2 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad)).normalized;
    }
}
