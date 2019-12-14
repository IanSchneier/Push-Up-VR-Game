using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRemove : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject); //free up some memory
    }

    private void OnTriggerEnter(Collider col)
    {
        Destroy(col.gameObject);
    }
}
