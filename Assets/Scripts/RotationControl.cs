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

    public GameObject Menu;

    // Flag to send calibrate signal (avoiding repeat calibrations)
    bool rotated = false;
    public enum Calibration {none, top, bottom};
    // Flag to change top limit ping value
    internal static Calibration CalibrationMode = Calibration.none;
    // Pointer to state machine object
    public GameObject MainStateMachine;
    // Pointer to state machine script
    private StateMachine GameState;

    private Pointer point;

    void Start()
    {
        point = new Pointer(GetComponent<LineRenderer>());
        // Connect to statemachine
        GameState = MainStateMachine.GetComponent<StateMachine>();
        CalibrationMode = Calibration.none;

        targetLayer = 1 << RaycastLayer;
    }

    void Update()
    {
        RaycastHit hit;

        //float y_rot = gameObject.GetComponent<Camera>().transform.eulerAngles.y;
        //Menu.transform.eulerAngles.SetY(y_rot);


        if (StateMachine.GameState())
        {
            point.Dim();
        }
        else
        {
            point.Show();

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, targetLayer))
            {
                if (hit.collider.gameObject == LeftText && !rotated)
                {
                    LeftText.GetComponent<MenuPanelUpdate>().InstructionComplete();
                    // Set calibration mode to set the lower limit
                    CalibrationMode = Calibration.bottom;
                    // Set flag to prevent repeat calibrations
                    rotated = true;
                }
                if (hit.collider.gameObject == RightText && !rotated)
                {
                    RightText.GetComponent<MenuPanelUpdate>().InstructionComplete();
                    // Set calibration mode to set the upper limit
                    CalibrationMode = Calibration.top;
                    // Set flag to prevent repeat calibrations
                    rotated = true;
                }
                else if (hit.collider.gameObject == TopText && !rotated)
                {
                    // Hide Line renderer
                    point.StartDim();
                    // Change state to begin playing the game
                    GameState.StartGame();
                    // Set flag to prevent game starting again
                    rotated = true;
                }
            }
            else
                rotated = false;
        }
    }

    internal class Pointer
    {
        private float WidthLossPerSecond = 0.0001f;
        // Flag to dim the line renderer
        private bool dim = false;
        private LineRenderer LR;
        // Original width
        private float OGwidth;
        public Pointer(LineRenderer lr, float dr = 0.01f)
        {
            LR = lr;
            WidthLossPerSecond = dr;
            OGwidth = LR.widthMultiplier;
            LR.enabled = true;
        }

        public void Show()
        {
            LR.enabled = true;
            LR.widthMultiplier = OGwidth;
        }

        public void StartDim()
        {
            dim = true;
        }

        public void Dim()
        {
            if (dim)
            {
                LR.widthMultiplier -= WidthLossPerSecond;
                //CG.alpha -= AlphaLossPerSecond;
                if (LR.widthMultiplier <= 0)
                {
                    LR.widthMultiplier = 0;
                    LR.enabled = false;
                    dim = false;
                }
            }
        }
    }
}

