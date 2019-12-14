using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabPartGenerate : MonoBehaviour
{
    private GameObject newChild;
    //private GameObject SpawnObject;
    public GameObject[] SpawnObjects;
    // Start is called before the first frame update
    void Start()
    {
        newChild = Instantiate(
            SpawnObjects[Random.Range(0, SpawnObjects.Length)], 
            transform.position, 
            AngleStuff.EulerToQuaterion(
                0,
                0,
                Random.Range(0, 360)
                )
            );
        newChild.transform.SetParent(transform);
    }
}


public static class AngleStuff
{
    public static Quaternion EulerToQuaterion(float yaw, float pitch, float roll)
    {
        yaw *= Mathf.Deg2Rad;
        pitch *= Mathf.Deg2Rad;
        roll *= Mathf.Deg2Rad;

        double yawOver2 = yaw * 0.5f;
        float cosYawOver2 = (float)System.Math.Cos(yawOver2);
        float sinYawOver2 = (float)System.Math.Sin(yawOver2);
        double pitchOver2 = pitch * 0.5f;
        float cosPitchOver2 = (float)System.Math.Cos(pitchOver2);
        float sinPitchOver2 = (float)System.Math.Sin(pitchOver2);
        double rollOver2 = roll * 0.5f;
        float cosRollOver2 = (float)System.Math.Cos(rollOver2);
        float sinRollOver2 = (float)System.Math.Sin(rollOver2);
        Quaternion result;
        result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }
}

