using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ArduinoBluetoothAPI;

public class ArduinoControl : MonoBehaviour
{
    public float Velocity;
    public float MoveTime;

    // Camera that will be used for game
    public Camera GameCamera;
    private BluetoothHelper helper;
    private string deviceName;
    string BTMessage;

    public int UpperLimitPing;
    public int LowerLimitPing;


    private float screen_range_min;
    private float screen_range_max;

    string posy_str;
    string ping_str;
    int ping;
    // Variable to hold vertical position of playerobject 
    private float posz;


    private int avg_total;
    private int avg_cnt;
    private int avg_cnt_max;
    private int avg_ping;
    private RotationControl.Calibration dir;
    public int PowerOfTwo = 4;


    Rigidbody m_Rigidbody;



    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        deviceName = "RNBT-9BF1";
        // Set min and max of localized coordinates
        //TODO: programatically replace hardcoded values
        screen_range_max = 4f;
        screen_range_min = -4f;
        // Setup orientation
        dir = RotationControl.Calibration.none;
        // Setup measurement averaging
        avg_total = avg_cnt = avg_ping = 0;
        avg_cnt_max = 1 << PowerOfTwo;


        try
        {
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.OnDataReceived += OnDataReceived;
            helper.setTerminatorBasedStream("\r\n");

            if (!helper.IsBluetoothEnabled())
                helper.EnableBluetooth(true);

            if (helper.isDevicePaired())
                helper.Connect();

        }
        catch(BluetoothHelper.BlueToothNotEnabledException e)
        {
            Debug.Log(e);
        }
        catch (BluetoothHelper.BlueToothNotReadyException e)
        {
            Debug.Log(e);
        }
        catch (BluetoothHelper.BlueToothNotSupportedException e)
        {
            Debug.Log(e);
        }
    }

    private void FixedUpdate()
    {
        if (StateMachine.GameState())
        {
            float vel = (posz - transform.position.y)/Time.deltaTime;
            float newPosition = Mathf.SmoothDamp(transform.position.z, posz, ref vel, Time.deltaTime);
            //float newPosition = Mathf.SmoothDamp(transform.position.x, posy, ref Velocity, MoveTime);
            //float newPosition = Mathf.SmoothDamp(transform.position.x, posy, ref Velocity, MoveTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, newPosition);
        }
    }

    void Update()
    {
        
        if (RotationControl.CalibrationMode > RotationControl.Calibration.none)
            StartCalibration();
    }

    void LateUpdate()
    {
        //m_Rigidbody.velocity = transform.up * (posy - transform.position.y);
        //transform.position = new Vector3(posx, posy, posz);
        
    }

    void OnConnected()
    {
        helper.StartListening();
        helper.SendData("B");
    }

    void OnConnectionFailed()
    {
        Debug.Log("Connection failed");
    }

    void OnDataReceived()
    {
        ping_str = helper.Read();
        ping = Convert.ToInt32(ping_str);

        avg_total += ping;
        avg_cnt++;
        if (avg_cnt >= avg_cnt_max)
        {
            avg_ping = avg_total>>PowerOfTwo;
            if (dir > 0)
            {
                FinishCalibration();
            }
            else
            {
                // Confine new position to predefined min and max
                ping = Math.Max(Math.Min(avg_ping, UpperLimitPing), LowerLimitPing);
                // Translate to screen position
                posz = map(ping, LowerLimitPing, UpperLimitPing, screen_range_min, screen_range_max);
                //posy = Mathf.Max(screen_range_min, posy);
                posy_str = posz.ToString();
            }
            // Reset values
            avg_cnt = avg_ping = avg_total = 0;
        }
    }

    void OnDestroy()
    {
        if (helper != null)
            helper.Disconnect();
    }
    /*
    void OnGUI()
    {
        
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(w >> 1, h, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.5f, 0.3f, 1.0f);

        GUI.Label(new Rect(w *3/ 4, h * 1 / 3, w, h * 2 / 100), posy_str, style);

        GUI.Label(new Rect(w *3/4, h * 2 / 3, w, h * 2 / 100), ping.ToString(), style);
        if(dir == RotationControl.Calibration.top)
            GUI.Label(new Rect(w / 4, h * 1 / 2, w, h * 2 / 100), "calibrating top", style);
        else if (dir == RotationControl.Calibration.bottom)
            GUI.Label(new Rect(w / 4, h * 1 / 2, w, h * 2 / 100), "calibrating bottom", style);
    }
    */
    void StartCalibration()
    {
        avg_cnt = 0;
        avg_total = 0;
        dir = RotationControl.CalibrationMode;
        //Set the mode to none to prevent repeat
        RotationControl.CalibrationMode = RotationControl.Calibration.none;
    }

    void FinishCalibration()
    {
        switch(dir)
        {
            case RotationControl.Calibration.top:
                UpperLimitPing = avg_ping;
                break;
            case RotationControl.Calibration.bottom:
                LowerLimitPing = avg_ping;
                break;
        }
        dir = RotationControl.Calibration.none;
    }

    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


}

public static class ExtensionMethods
{
    public static int RoundOff(this int i)
    {
        return ((int)Math.Round(i / 10.0)) * 10;
    }
}