using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
  VR version of script for running the simulation and manipulating certain
  aspects of it. Added functions for controlling playback speed of animation
  (left and right on the touchpad of the right controller), rewinding/playing
  (up and down on the touchpad of the right controller) and for toggling the
  transparency of the brain and skull on and off (trigger on the back of the
  left controller). Some additional functions exist in the script but were not
  working as intended before user tests were held and therefore not used.
*/

public class VRSimulator : BaseSimulator
{
    private float densityVisSpeed = 0.2f;

    
    [Header("LEFT VR Settings")]
    public Transform leftVRController;
    public GameObject markerSphere;

    public float LleftSens = -0.8f;
    public float LrightSens = 0.7f;
    public float LupSens = 0.6f;
    public float LdownSens = -0.6f;
    

    [SerializeField] private InputActionReference transparencyController = null;
    [SerializeField] private InputActionReference toggleTransparent = null;



    [Header("RIGHT VR Settings")]
    public Transform rightVRController;


    public float RleftSens = -0.7f;
    public float RrightSens = 0.7f;
    public float RupSens = 0.6f;
    public float RdownSens = -0.6f;
    private float prePlayPauseBool = 0;
    private float preRtrackpad = 0;

    private float currTime;

    [SerializeField] private InputActionReference togglePlay = null;
    [SerializeField] private InputActionReference timeController = null;
    [SerializeField] private InputActionReference annotatePoint = null;
    [SerializeField] private InputActionReference RtrackClick = null;


    private bool transparencyToggleInProgress = false; // Todo: Is this actualy unused/superficient?

    override protected void handleInput()
    {
        // Handle VR input
        Vector2 timeInputVal = timeController.action.ReadValue<Vector2>();
        Vector2 transparencyInputVal = transparencyController.action.ReadValue<Vector2>();
        float toggleVal = toggleTransparent.action.ReadValue<float>();
        float playPauseBool = togglePlay.action.ReadValue<float>();
        float Rtrackpad = RtrackClick.action.ReadValue<float>();

        

        if (playPauseBool==1 && prePlayPauseBool==0)
        {
            if (paused == true)
            {
                paused = false;
            }
            else
            {
                paused = true;
            }
            
        }
        prePlayPauseBool = playPauseBool;

        if (Input.GetKeyDown(KeyCode.Q))
        {

            ToggleTransparency();
        }

        if (toggleVal > 0.8f && !transparencyToggleInProgress) //If we got input to toggle transparency
        {
            //Debug.Log(toggleVal);
            transparencyToggleInProgress = true;
            ToggleTransparency(); // Toggle transparency instantly
        }
        else if (toggleVal < 0.1f && transparencyToggleInProgress) // If we no longer have input to toggle transparency
        {
            transparencyToggleInProgress = false;
        }

        ///////////////////////////////////////////////////////////////////////

        if (timeInputVal != Vector2.zero && Rtrackpad == 1 && preRtrackpad == 0 &&
            timeInputVal.y < RupSens && timeInputVal.y > RdownSens && timeInputVal.x < RrightSens && timeInputVal.x > RleftSens)
        {
            if (paused)
            {
                paused = false;
            } else
            {
                paused = true;
            }
        }


        if (timeInputVal != Vector2.zero && paused && preRtrackpad == 0 && Rtrackpad == 1) // If there is input controlling the playback
        {
            //Debug.Log("TIME");

            if (timeInputVal.y > RupSens  && index < maxFileSize - 10) // If there is input to reduce the playback speed
            {
                foreach (Data data in dataList)
                {

                    simulateData(data);
                    if (applyPathTrace) visualizePathTrace(data);
                }
                index++;
                if (FrameStuff[0])
                {
                    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
                }
            }

            else if (timeInputVal.y < RdownSens && index > 0) // If there is input to reduce the playback speed
            {
                rewind = true;
                forward = false;
                foreach (Data data in dataList)
                {
                    simulateData(data);
                    if (applyPathTrace) visualizePathTrace(data);
                }
                index--;
                if (FrameStuff[0])
                {
                    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
                }
                rewind = false;
                forward = true;
            }
        }



        //////////////////////////////////////////////////////////////



        if (timeInputVal != Vector2.zero && Rtrackpad == 1 && preRtrackpad == 0)
        {
            float currTime = Time.time;
        }

        if (timeInputVal != Vector2.zero && Rtrackpad == 1) // If there is input controlling the playback
        {
            
            
            //Debug.Log("TIME");

            if (timeInputVal.x > RrightSens && Time.time > currTime + 0.1 && index < maxFileSize - 10) // If there is input to reduce the playback speed
            {
                Debug.Log("INNIT");
                foreach (Data data in dataList)
                {

                    simulateData(data);
                    if (applyPathTrace) visualizePathTrace(data);
                }
                index++;
                if (FrameStuff[0])
                {
                    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
                }
            }

            else if (timeInputVal.x < RleftSens && Time.time > currTime + 0.1 && index > 0) // If there is input to reduce the playback speed
            {
                rewind = true;
                forward = false;
                foreach (Data data in dataList)
                {
                    simulateData(data);
                    if (applyPathTrace) visualizePathTrace(data);
                }
                index--;
                if (FrameStuff[0])
                {
                    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
                }
                rewind = false;
                forward = true;
            }
        }
        preRtrackpad = Rtrackpad;





        /*
        if (timeInputVal != Vector2.zero) // If there is input controlling the playback
        {
            if (applySpaceTimeDensity)
            {
                if (timeInputVal.x > rightSens) // If there is input to increase the left visbility cutoff
                {
                    Vector2 visWindow = volObjScript.GetVisibilityWindow();
                    visWindow.x += densityVisSpeed * Time.deltaTime;
                    if (visWindow.x > visWindow.y) visWindow.x = visWindow.y;
                    volObjScript.SetVisibilityWindow(visWindow);
                }
                else if (timeInputVal.x < leftSens) // If there is input to decrease the left visbility window cutoff
                {
                    Vector2 visWindow = volObjScript.GetVisibilityWindow();
                    visWindow.x -= densityVisSpeed * Time.deltaTime;
                    if (visWindow.x < 0.0f) visWindow.x = 0;
                    volObjScript.SetVisibilityWindow(visWindow);
                }

                if (timeInputVal.y > upSens) // If there is input to increase the right visibility window cutoff
                {
                    Vector2 visWindow = volObjScript.GetVisibilityWindow();
                    visWindow.y += densityVisSpeed * Time.deltaTime;
                    if (visWindow.y > 1.0f) visWindow.y = 1;
                    volObjScript.SetVisibilityWindow(visWindow);
                }
                else if (timeInputVal.y < downSens) // If there is input to decrease the right visibility window cutoff
                {
                    Vector2 visWindow = volObjScript.GetVisibilityWindow();
                    visWindow.y -= densityVisSpeed * Time.deltaTime;
                    if (visWindow.y < visWindow.x) visWindow.y = visWindow.x;
                    volObjScript.SetVisibilityWindow(visWindow);
                }
            }
            else
            {
                if (timeInputVal.x > rightSens) // If there is input to switch to forward playback
                {
                    rewind = false;
                    forward = true;
                }
                else if (timeInputVal.x < leftSens) // If there is input to rewind
                {
                    rewind = true;
                    forward = false;
                }

                if (timeInputVal.y > upSens) // If there is input to increase the playback speed
                {
                    if (playBackSpeed < maxPlaybackSpeed)
                    {
                        if (paused)
                        {
                            paused = false;
                        }
                        playBackSpeed += maxPlaybackSpeed / 2f * timeInputVal.y * Time.deltaTime;
                    }
                }
                else if (timeInputVal.y < downSens) // If there is input to reduce the playback speed
                {
                    if (playBackSpeed > 1f)
                    {
                        playBackSpeed -= maxPlaybackSpeed / 2f * -timeInputVal.y * Time.deltaTime;
                    }
                    else
                    {
                        paused = true;
                    }

                    if (playBackSpeed < 0.1f && !paused)
                    {
                        playBackSpeed = 0.5f;
                    }


                    Debug.Log(playBackSpeed);
                }
            }
            
            //Todo: The input values might need to be adjusted based on hardware
            if (timeInputVal.x > 0.7) // If there is input to switch to forward playback
            {
                rewind = false;
                forward = true;
            }
            else if (timeInputVal.x < -0.8f) // If there is input to rewind
            {
                rewind = true;
                forward = false;
            }

            if (timeInputVal.y > 0.6f) // If there is input to increase the playback speed
            {
                if (playBackSpeed < maxPlaybackSpeed)
                {
                    if (paused)
                    {
                        paused = false;
                    }
                    playBackSpeed += maxPlaybackSpeed / 2f * timeInputVal.y * Time.deltaTime;
                }
            }
            else if (timeInputVal.y < -0.6f) // If there is input to reduce the playback speed
            {
                if (playBackSpeed > 1f)
                {
                    playBackSpeed -= maxPlaybackSpeed / 2f * -timeInputVal.y * Time.deltaTime;
                }
                else
                {
                    paused = true;
                }

                if (playBackSpeed < 0.1f && !paused)
                {
                    playBackSpeed = 0.5f;
                }


                Debug.Log(playBackSpeed);
            }

            //Debug.Log("Forward: " + forward + "| Backward: " + rewind + "| Playback speed: " + playBackSpeed);

        }
        Debug.Log("b " + toggleVal);

        if (toggleVal > 0.8f && !transparencyToggleInProgress) //If we got input to toggle transparency
        {
            Debug.Log(toggleVal);
            transparencyToggleInProgress = true;
            ToggleTransparency(); // Toggle transparency instantly
        }
        else if (toggleVal < 0.1f && transparencyToggleInProgress) // If we no longer have input to toggle transparency
        {
            transparencyToggleInProgress = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Invoke(nameof(RestartScene), 1f); // Soft restart the scene (method in this script)
        }
        */
    }
}
