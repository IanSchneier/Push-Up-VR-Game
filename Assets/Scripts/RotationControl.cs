using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationControl : MonoBehaviour
{
    public int RaycastLayer;
    private int targetLayer;
    //Left Text box
    public GameObject LeftText;
    //Right Text box
    public GameObject RightText;

    public GameObject TopText;

    // Flag to send calibrate signal (avoiding repeat calibrations)
    bool rotated = false;
    public enum Calibration {none, top, bottom};
    // Flag to change top limit ping value
    internal static Calibration CalibrationMode = Calibration.none;
    // Pointer to state machine object
    public GameObject MainStateMachine;
    // Pointer to state machine script
    private StateMachine GameState;
    private string debug;
    void Start()
    {
        // Connect to statemachine
        GameState = MainStateMachine.GetComponent<StateMachine>();
        CalibrationMode = Calibration.none;

        targetLayer = 1 << RaycastLayer;
    }

    void Update()
    {
        RaycastHit hit;
        if (StateMachine.GameState())
        {

        }
        else
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetLayer))
            {
                if (hit.collider.gameObject == LeftText && !rotated)
                {
                    LeftText.GetComponent<MenuPanelUpdate>().InstructionComplete();
                    // Set calibration mode to set the lower limit
                    debug = "callibrating left";
                    CalibrationMode = Calibration.bottom;
                    // Set flag to prevent repeat calibrations
                    rotated = true;
                }
                if (hit.collider.gameObject == RightText && !rotated)
                {
                    RightText.GetComponent<MenuPanelUpdate>().InstructionComplete();
                    // Set calibration mode to set the upper limit
                    CalibrationMode = Calibration.top;
                    debug = "callibrating right";
                    // Set flag to prevent repeat calibrations
                    rotated = true;
                }
                else if (hit.collider.gameObject == TopText && !rotated)
                {
                    // Change state to begin playing the game
                    GameState.StartGame();
                    debug = "starting game";
                    // Set flag to prevent game starting again
                    rotated = true;
                }
            }
            else
                rotated = false;
        }
        


        /*if ((Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetLayer)) && !rotated)
        {
            if (hit.collider.gameObject.tag == "MenuLeft")
            {
                //LeftText.GetComponent<TextUpdate>().InstructionComplete();
                // Set calibration mode to set the lower limit
                debug = "callibrating left";
                CalibrationMode = Calibration.bottom;
                // Set flag to prevent repeat calibrations
                rotated = true;
            }
            else if(hit.collider.gameObject.tag == "MenuRight")
            {
                //RightText.GetComponent<TextUpdate>().InstructionComplete();
                // Set calibration mode to set the upper limit
                CalibrationMode = Calibration.top;
                debug = "callibrating right";
                // Set flag to prevent repeat calibrations
                rotated = true;
            }
            else if(hit.collider.gameObject.tag == "MenuTop")
            {
                // Change state to begin playing the game
                GameState.StartGame();
                debug = "starting game";
                // Set flag to prevent game starting again
                rotated = true;
            }
        }*/
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.1942594f, 1.0f, 0.0f, 1.0f);

        GUI.Label(new Rect(w >> 1, h * 1 / 3, w, h * 2 / 100), debug, style);
    }
}