using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class StateMachine : MonoBehaviour
{
    public float FlashEffectDecay;
    public GameObject PlayerObject;
    private PlayerObject Player;
    // Boolean to switch between the two game modes:
    // themenu (false) - when you calibrate headset and start
    // playing (true)  - when you playing the game
    private static bool playing_game;
    private static Stopwatch GameTime;

    //Left Text box
    public Transform LeftText;
    //Right Text box
    public Transform RightText;
    //Top text box
    public Transform TopText;

    // Speed to transition textboxes offscreen
    public float Velocity = 5f;
    // time it takes to move them off screen
    public float MoveTime = 0.5f;
    // destinatino offsets of the textboxes from their respective locaitons
    public float DestinationOffset = 6f;

    private GameMenu Interface;

    private GameOver Stop;

    private GameMusic Soundtrack;
    void Start()
    {
        AudioSource[] sounds = GetComponents<AudioSource>();
        playing_game = false;
        // Create new gameover transition instance
        Stop = new GameOver(
            gameObject.GetComponent<CanvasGroup>(),
            FlashEffectDecay, 
            sounds[0]
            );

        // Create game menu interactin instance
        Interface = new GameMenu(
            LeftText.transform,
            RightText.transform,
            TopText.transform,
            Velocity,
            MoveTime
            );

        //Create new soundtrack instance
        Soundtrack = new GameMusic(FlashEffectDecay, sounds[1]);

        Player = PlayerObject.GetComponent<PlayerObject>();       
    }

    private void Update()
    {
        if (Input.GetKey("up") && !playing_game)
        {
            StartGame();
        }

        if (playing_game)
        {
            Soundtrack.AdjustVolume();
            Interface.MoveMenu();
        }
        else
            Stop.GameOverUpdate();
    }

    public void StartGame()
    {
        if (!playing_game) // Do nothing if game is already going
        {
            playing_game = true;
            //Begin timer from when game started
            GameTime = Stopwatch.StartNew();

            Player.MoveToStartPosition();
            Interface.CloseMenu();
            Soundtrack.UnMute();
            
        }
    }

    public void StopGame()
    {
        if (playing_game) // procede to game over screen
        {
            // Change gamemode flag to force other scripts to change behavior
            playing_game = false;
            // Stop soundtrack
            Soundtrack.Mute();
            // re-setup menu
            Interface.ShowMenu();
            // Play the gameover transition effect
            Stop.Boom();
            // Stop the game clock
            GameTime.Stop();
            // Get all existing game objects with the "Spawned" tag
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Spawned");
            // Remove those objects
            foreach (GameObject obs in obstacles)
                Destroy(obs);
            // Reset instructions
            LeftText.GetComponent<MenuPanelUpdate>().InstructionReset();
            RightText.GetComponent<MenuPanelUpdate>().InstructionReset();
        }
    }

    internal static bool GameState()
    {
        return playing_game;
    }

    internal static float TimeSinceGameStart()
    {
        return (float)GameTime.Elapsed.TotalMinutes;
    }
}

internal class GameOver
{
    private CanvasGroup CG;
    private float AlphaLossPerSecond;
    private bool flash = false;
    private AudioSource player;
    public GameOver(CanvasGroup cg, float dr, AudioSource plyr)
    {
        CG = cg;
        AlphaLossPerSecond = dr;
        player = plyr;
    }

    public void GameOverUpdate()
    {
        if (flash)
        {
            CG.alpha -= AlphaLossPerSecond;
            player.volume -= AlphaLossPerSecond;
            if (CG.alpha <= 0)
            {
                CG.alpha = 0;
                flash = false;
                player.Stop();
                player.volume = 1.0f;
            }
        }
    }

    public void Boom()
    {
        flash = true;
        CG.alpha = 1;
        player.Play();
    }
}

internal class GameMusic
{
    private AudioSource player;
    private float volume_increase_rate;
    private bool increaseVolume = false;

    public GameMusic(float volume_chng, AudioSource src)
    {
        player = src;
        volume_increase_rate = volume_chng;
        player.volume = 0.0f;
        player.loop = true;
    }

    public void AdjustVolume()
    {
        if (increaseVolume && (player.volume < 1.0f))
        {
            player.volume += volume_increase_rate;
        }
    }

    public void UnMute()
    {
        increaseVolume = true;
    }

    public void Mute()
    {
        increaseVolume = false;
        player.volume = 0.0f;
    }
}

internal class GameMenu
{
    // time it takes to move them off screen
    private float MoveTime = 0.5f;
    // destinatino offsets of the textboxes from their respective locaitons
    public float DestinationOffset = 6f;
    // Velocity of left textbox
    private float VelocityL;
    // Velocity of right textbox
    private float VelocityR;
    // Velocity of top text box
    private float VelocityT;
    // Variables that contain the coordinates of the textboxes when in game menu
    private Vector3 initialLeft;
    private Vector3 initialRight;
    private Vector3 initialTop;
    // coordinate values that are sum of initial position plus offsets
    private float destLeft;
    private float destRight;
    private float destTop;

    private Transform PositionLeft;
    private Transform PositionRight;
    private Transform PositionTop;

    private bool move_left = false;
    private bool move_right = false;
    private bool move_top = false;

    public GameMenu(Transform initial_location_left, Transform initial_location_right, Transform initial_location_top, float move_speed = 5.0f, float travel_time = 0.5f)
    {
        PositionLeft = initial_location_left;
        PositionRight = initial_location_right;
        PositionTop = initial_location_top;
        // Save values of textboxes initial locations
        initialLeft = initial_location_left.position;
        initialRight = initial_location_right.position;
        initialTop = initial_location_top.position;
        // Get location coordinates of their offscreen positions
        destLeft = initialLeft.x - DestinationOffset;
        destRight = initialRight.x + DestinationOffset;
        destTop = initialTop.y + DestinationOffset;
        // Give all three velocities same initial value but will deviate when actually moving
        VelocityL = VelocityR = VelocityT = move_speed;

        MoveTime = travel_time;
    }


    public void CloseMenu()
    {
        move_left = true;
        move_right = true;
        move_top = true;
}

    public void ShowMenu()
    {
        //Reset positions of textboxes
        PositionLeft.position = initialLeft;
        PositionRight.position = initialRight;
        PositionTop.position = initialTop;
    }

    public void MoveMenu()
    {
        if (move_left)
        {
            // Get new position for left text
            float newLeftPosition = Mathf.SmoothDamp(PositionLeft.position.x, destLeft, ref VelocityL, MoveTime);
            //Move it
            PositionLeft.position = new Vector3(newLeftPosition, PositionLeft.position.y, PositionLeft.position.z);
            if (Mathf.Abs(PositionLeft.position.x - destLeft) < 0.01f)
            {
                PositionLeft.position = new Vector3(destLeft, PositionLeft.position.y, PositionLeft.position.z);
                move_left = false;
            }
        }

        if (move_right)
        {
            // Get new position for right text
            float newRightPosition = Mathf.SmoothDamp(PositionRight.position.x, destRight, ref VelocityR, MoveTime);
            PositionRight.position = new Vector3(newRightPosition, PositionRight.position.y, PositionRight.position.z);
            if (destRight - PositionRight.position.x < 0.01f)
            {
                PositionRight.position = new Vector3(destRight, PositionRight.position.y, PositionRight.position.z);
                move_right = false;
            }
        }

        if (move_top)
        {
            // get new position for top text
            float newTopPosition = Mathf.SmoothDamp(PositionTop.position.y, destTop, ref VelocityT, MoveTime);
            PositionTop.position = new Vector3(PositionTop.position.x, newTopPosition, PositionTop.position.z);
            if (destTop - PositionTop.position.y < 0.01f)
            {
                PositionTop.position = new Vector3(PositionTop.position.x, destTop, PositionTop.position.z);
                move_top = false;
            }
        }
    }
    
}