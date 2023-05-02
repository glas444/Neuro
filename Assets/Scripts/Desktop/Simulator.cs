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

    override protected void checkVR()
    {
        isVR = false;
    }

    override protected void handleInput()
    {

        if (Input.GetKeyDown("i"))
        {
            Debug.Log(index);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            float timeRes = Time.time-timeStart;

            string res = recordingIndex + "," + marker.transform.position.x + "," + marker.transform.position.y + "," + marker.transform.position.z + "," + marker.transform.localScale.x + ","+ timeRes + "," +nrMoveSteps+ "," +nrMarkerSteps + "," + nrTimeSteps;
            using (StreamWriter writer = new StreamWriter("Result.txt",true))
            {
                writer.WriteLine(res);
            }
            init();
        }


        // Toggle transparency on and off
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTransparency();
        }


        // Pause/Play the animation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            nrTimeSteps++;
            playPause();
        }


        //-------------------------------- RIGHT --------------------------------

        // Right arrow key plays the animation forward in time
        if (Input.GetKeyDown(KeyCode.RightArrow) && !applySpaceTimeDensity)
        {
            nrTimeSteps++;
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
            visualiseData();

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
            visualiseData();
        }

        //-------------------------------- LEFT --------------------------------

        // Left arrow key plays the animation forward in time
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !applySpaceTimeDensity)
        {
            nrTimeSteps++;
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
            visualiseData();

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
            visualiseData();
        }





        //-------------------------------- RIGHT --------------------------------

        // Step one frame forward in time
        if (Input.GetKeyDown(".") && !applySpaceTimeDensity)
        {
            nrTimeSteps++;
            if (index < maxFileSize - 1)
            {
                index++;
            }
            visualiseData();
        }




        // Step one frame back in time
        if (Input.GetKeyDown(",") && !applySpaceTimeDensity)
        {
            nrTimeSteps++;
            if (index > 0)
            {
                index--;
            }

            visualiseData();
        }








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
