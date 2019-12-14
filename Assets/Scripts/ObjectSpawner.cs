using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private GameObject SpawnObject;
    public GameObject[] SpawnObjects;

    public float timeMin = 3f;
    public float timeMax = 7f;
    public float Scale = 10f;

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
            float y = Random.Range(-SpawnObject.transform.localScale.y * Scale, SpawnObject.transform.localScale.y * Scale);
            GameObject go = Instantiate(SpawnObject, this.transform.position + new Vector3(0, y, 0), Quaternion.identity) as GameObject;
        }
        Invoke("Spawn", Random.Range(timeMin, timeMax));
    }
}
