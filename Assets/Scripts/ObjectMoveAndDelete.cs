using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectMoveAndDelete: MonoBehaviour
{
    public float Speed = 2.3f;
    public float MaxSpeed = 12f;
    public float DeletionLocation = -8.0f;

    private float actual_speed;

    void Start()
    {
        if (StateMachine.GameState())
        {
            // Get time in minutes since game started
            float time_from_start = Speed + StateMachine.TimeSinceGameStart();
            // Set velocity based on time since game started
            actual_speed = Mathf.Min(time_from_start, MaxSpeed);
        }
        
    }

    void Update()
    {
        if ((transform.localPosition.x + transform.localScale.x) < DeletionLocation)
        {
            Destroy(transform.gameObject);
        }
        transform.Translate(Vector3.left * actual_speed * Time.deltaTime);
    }
}
