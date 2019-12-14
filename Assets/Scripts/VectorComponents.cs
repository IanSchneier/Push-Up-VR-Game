using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class VectorComponents
{
    public static void SetX(this Transform t, float newYPos)
    {
        var pos = t.position;
        pos.x = newYPos;
        t.position = pos;
    }

    public static void SetX(this Vector3 t, float newYPos)
    {
        var pos = t;
        pos.x = newYPos;
        t = pos;
    }

    public static void SetY(this Transform t, float newYPos)
    {
        var pos = t.position;
        pos.y = newYPos;
        t.position = pos;
    }

    public static void SetY(this Vector3 t, float newYPos)
    {
        var pos = t;
        pos.y = newYPos;
        t = pos;
    }

    public static void SetZ(this Transform t, float newYPos)
    {
        var pos = t.position;
        pos.y = newYPos;
        t.position = pos;
    }

    public static void SetZ(this Vector3 t, float newYPos)
    {
        var pos = t;
        pos.y = newYPos;
        t = pos;
    }
}
