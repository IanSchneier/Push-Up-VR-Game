using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private GameObject SpawnObject;
    public GameObject[] SpawnObjects;

    public float timeMin = 3f;
    public float timeMax = 7f;
    public float Scale = 4f;
    public float Offset = 0;

    // Use this for initialization
    void Start()
    {
        SpawnObject = SpawnObjects[Random.Range(0, SpawnObjects.Length)];
        Spawn();
    }

    void Spawn()
    {
        if (StateMachine.GameState())
        { 
            //random y position
            float z = Random.Range(-Scale, Scale) + Offset;
            GameObject go = Instantiate(SpawnObject, transform.position + new Vector3(0, 0, z), Quaternion.identity) as GameObject;
        }
        Invoke("Spawn", Random.Range(timeMin, timeMax));
    }
}
