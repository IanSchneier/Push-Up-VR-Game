using System.Collections;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    // Pointer to position of game ojbect where points will be displayed at
    public Transform ScoreLocation;
    // Pointer to state machine object
    public GameObject MainStateMachine;
    //Location player object should at start of game
    public Vector3 OnscreenPosition;
    //Location of player object before game start (where it's located during development)
    private Vector3 OffscreenPosition;
    // Flag to signal that player pbject shoudl move to play mode position
    private bool move_to_screen = false;
    // Time to wait for other stuff to leave screen
    public float WaitDelay = 0.3f;
    // Speed object moves onto screen
    public float VelocityX = 5f;
    // Time it takes to get on screen
    public float MoveTimeX = 0.5f;
    // Pointer to state machine script
    private StateMachine GameState;
    // Flag to start move after a predetermined duration
    private bool delay_move = false;
    // Var to keep track of the number of obstacles avoided
    private int score;
    // Variables to keep screen dimensions
    private int w = Screen.width;
    private int h = Screen.height;
    // Style 
    private GUIStyle style;

    void Start()
    {
        GameState = MainStateMachine.GetComponent<StateMachine>();
        // set global to initial value to return to when game over
        OffscreenPosition = transform.position;
        // Setup GUI stuff
        style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = h * 2 / 50
        };
        style.normal.textColor = new Color(0.1942594f, 1.0f, 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (delay_move)
            StartCoroutine(OnscreenDelay());

        if (move_to_screen)
        {
            float newPosition = Mathf.SmoothDamp(transform.position.x, OnscreenPosition.x, ref VelocityX, MoveTimeX);
            transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
            // If ship is close enough to destination that player can't tell that it isn't then
            // set it to destination position and stop moving it.
            if (Mathf.Abs(OnscreenPosition.x - transform.position.x) < 0.01f)
            {
                transform.position = OnscreenPosition;
                move_to_screen = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            //Player lost the game
            GameState.StopGame();
            // Move ship back offscreen;
            StopPosition();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        score++;
    }

    void OnGUI()
    {
        Vector2 playerpoint = Camera.main.WorldToScreenPoint(
            new Vector2(
                ScoreLocation.position.x,
                ScoreLocation.position.y
                )
            );
        if(StateMachine.GameState())
            GUI.Label(
                new Rect(playerpoint.x, h - playerpoint.y, w, h * 2 / 100),
                (score > 0) ? score.ToString() : "0",
                style
                );
    }

    private IEnumerator OnscreenDelay()
    {
        delay_move = false;
        yield return new WaitForSeconds(WaitDelay);
        move_to_screen = true;
    }

    public void MoveToStartPosition()
    {
        delay_move = true;
    }

    void StopPosition()
    {
        move_to_screen = false;
        transform.position = OffscreenPosition;
        // Reset score
        score = 0;
    }
}
