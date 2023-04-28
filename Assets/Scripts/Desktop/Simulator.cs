using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;

/*
  Desktop version of script for running the simulation and manipulating certain
  aspects of it. Added functions for controlling playback speed of animation
  (up and down arrow keys), rewinding/playing (left and right arrows keys) and
  for toggling the transparency of the brain and skull on and off (Q key). Some
  additional functions exist in the script but were not working as intended
  before user tests were held and therefore not used.
*/


public class Simulator : BaseSimulator
{
    private float currTime1;
    private float currTimeSpeedUp;
    private float fastforwardspeed = 0.04f;
    private bool firstspeed = false;
    private float densityVisSpeed = 0.2f;



    override protected void handleInput()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {

        }

        //Debug.Log("PC index= " + index);
        // Toggle transparency on and off with the Q key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTransparency();
        }
        if (transparencyEnabled) // Check if we want to adjust the transparency (Disabled?)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                // AdjustTransparency(10);
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                //AdjustTransparency(-10);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playPause();

        }


        // Right arrow key plays the animation forward in time
        if (Input.GetKeyDown(KeyCode.RightArrow) && !applySpaceTimeDensity)
        {
            currTime1 = Time.time;
            firstspeed = false;

            if (index < maxFileSize-1)
            {
                index += 5;
            }
            if (index >= maxFileSize-1)
            {
                index = maxFileSize - 1;
            }
            foreach (Data data in dataList)
            {
                simulateData(data);
                visualizePathTrace(data);
            }

        }
        if (Input.GetKey(KeyCode.RightArrow) && Time.time > currTime1 + 0.5 && index < maxFileSize-1)
        {
            //first time entering GetKey function, reset currTimeSpeedUp timer
            if (!firstspeed)
            {
                firstspeed = true;

                if (index < maxFileSize - 1)
                {
                    index++;
                }
                currTimeSpeedUp = Time.time;
            } 
            else if( Time.time > currTimeSpeedUp + fastforwardspeed)
            {
                if (index < maxFileSize - 1)
                {
                    index++;
                }
                currTimeSpeedUp = Time.time; //reset the time to fast forward
            }
            foreach (Data data in dataList)
            {
                simulateData(data);
                visualizePathTrace(data);
            }
        }



        // Left arrow key plays the animation forward in time
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !applySpaceTimeDensity)
        {
            currTime1 = Time.time;
            firstspeed = false;

            if (index > 0)
            {
                index -= 5;
            }
            if (index < 0)
            {
                index = 0;
            }
            foreach (Data data in dataList)
            {
                simulateData(data);
                visualizePathTrace(data);
            }

        }
        if (Input.GetKey(KeyCode.LeftArrow) && Time.time > currTime1 + 0.5 && index > 0)
        {
            //first time entering GetKey function, reset currTimeSpeedUp timer
            if (!firstspeed)
            {
                firstspeed = true;

                if (index > 0)
                {
                    index--;
                }
                currTimeSpeedUp = Time.time;
            }
            else if (Time.time > currTimeSpeedUp + fastforwardspeed)
            {
                if (index > 0)
                {
                    index--;
                }
                currTimeSpeedUp = Time.time; //reset the time to fast forward
            }
            if (index < 0)
            {
                index = 0;
            }
            foreach (Data data in dataList)
            {
                simulateData(data);
                visualizePathTrace(data);
            }
        }




        // Right arrow key plays the animation forward in time
        if (Input.GetKeyDown(".") && !applySpaceTimeDensity)
        {
            if (index < maxFileSize - 1)
            {
                index++;
            }


            //if (FrameStuff[0])
            //{
            //FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
            //}
        }



        // Right arrow key plays the animation forward in time
        if (Input.GetKeyDown(",") && !applySpaceTimeDensity)
        {
            //rewind = true;
            //forward = false;

            if (index > 0)
            {
                index--;
            }

            //if (FrameStuff[0])
            //{
            //    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
            //}
            //rewind = false;
            //forward = true;

        }




        /*
        // Right arrow key plays the animation forward in time
        if (Input.GetKeyDown(KeyCode.RightArrow) && !applySpaceTimeDensity)
        {
            rewind = false;
            forward = true;
        }
        // Left arrow key rewinds the animation
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !applySpaceTimeDensity)
        {
            rewind = true;
            forward = false;
        }
        */



        /*
        // Left arrow key decreases the cut of value of the visibility window
        if (Input.GetKey(KeyCode.LeftArrow) && applySpaceTimeDensity)
        {
            Vector2 visWindow = volObjScript.GetVisibilityWindow();
            visWindow.x -= densityVisSpeed * Time.deltaTime;
            if (visWindow.x < 0.0f) visWindow.x = 0;
            volObjScript.SetVisibilityWindow(visWindow);
        }
        // Right arrow key increases the cut of value of the visibility window
        else if (Input.GetKey(KeyCode.RightArrow) && applySpaceTimeDensity)
        {
            Vector2 visWindow = volObjScript.GetVisibilityWindow();
            visWindow.x += densityVisSpeed * Time.deltaTime;
            if (visWindow.x > visWindow.y) visWindow.x = visWindow.y;
            volObjScript.SetVisibilityWindow(visWindow);
        }
        */





        /*
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //Down arrow key decreases the right cut off value of the visibility window
            if (applySpaceTimeDensity)
            {
                Vector2 visWindow = volObjScript.GetVisibilityWindow();
                visWindow.y -= densityVisSpeed * Time.deltaTime;
                if (visWindow.y < visWindow.x) visWindow.y = visWindow.x;
                volObjScript.SetVisibilityWindow(visWindow);
            }
            // Down arrow key reduces playback speed of the animation
            else
            {
                if (playBackSpeed > 0f)
                {
                    playBackSpeed -= maxPlaybackSpeed / 2f * Time.deltaTime;
                }
                else
                {
                    paused = true;
                }
            }
            
            if (playBackSpeed < 0.1f && !paused)
            {
                playBackSpeed = 0.5f;
            }
        }
        
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            //Up arrow key increases the right cut off value of the visibility window
            if (applySpaceTimeDensity)
            {
                Vector2 visWindow = volObjScript.GetVisibilityWindow();
                visWindow.y += densityVisSpeed * Time.deltaTime;
                if (visWindow.y > 1.0f) visWindow.y = 1;
                volObjScript.SetVisibilityWindow(visWindow);
            }
            else
            {
                // Up arrow key increases playback speed of the animation
                if (playBackSpeed < maxPlaybackSpeed)
                {
                    if (paused)
                    {
                        paused = false;
                    }
                    playBackSpeed += maxPlaybackSpeed / 2f * Time.deltaTime;
                }
            }
        }
        */
        // R key restarts scene

    }
}
