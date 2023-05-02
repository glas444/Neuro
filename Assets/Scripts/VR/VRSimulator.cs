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
    public GameObject markerSphere;

    [Header("LEFT VR Settings")]
    public Transform leftVRController;
    public GameObject leftController;

    public GameObject scaleSphere;
    public Material transparentMat;
    public Material solidMat;

    public Vector3 markerDist;
    public float markerSizeMin = 0.001f;
    public float markerSizeMax = 0.1f;

    public float LleftSens = -0.8f;
    public float LrightSens = 0.7f;
    public float LupSens = 0.6f;
    public float LdownSens = -0.6f;
    private float preLtrackpad = 0;

    public Vector3 scaleCubeDist;

    [SerializeField] private InputActionReference spaceController = null;

    [SerializeField] private InputActionReference toggleTransparent = null;
    [SerializeField] private InputActionReference LtrackClick = null;
    [SerializeField] private InputActionReference LtriggerZ = null;


    [Header("RIGHT VR Settings")]
    public Transform rightVRController;
    public GameObject rightController;
    private bool transparencyMarkerEnabled;

    public float RleftSens = -0.7f;
    public float RrightSens = 0.7f;
    public float RupSens = 0.6f;
    public float RdownSens = -0.6f;
    private float prePlayPauseBool = 0;
    private float preRtrackpad = 0;

    private float currTime;

    [SerializeField] private InputActionReference NextReturn = null;
    [SerializeField] private InputActionReference togglePlay = null;
    [SerializeField] private InputActionReference timeController = null;
    [SerializeField] private InputActionReference annotatePoint = null;
    [SerializeField] private InputActionReference RtrackClick = null;
    [SerializeField] private InputActionReference RtriggerZ = null;

    public float RtriggerVal;
    public float LtriggerVal;


    private bool transparencyToggleInProgress = false; // Todo: Is this actualy unused/superficient?

    private float triggerSens = 0.7f;
    private bool leftHit = false;
    private bool rightHit = false;
    public GameObject rightHTC;
    public GameObject leftHTC;
    private bool RjustGrabbed;
    private bool LjustGrabbed;
    private Vector3 scaleCubeRotation;

    Vector3 initialHandPosition1; // first hand position
    Vector3 initialHandPosition2; // second hand position
    Quaternion initialObjectRotation; // grabbed object rotation
    Vector3 initialObjectScale; // grabbed object scale
    Vector3 initialObjectDirection; // direction of object to midpoint of both hands
    Vector3 initialObjectDirectionRight;
    public bool twoHandGrab = false; // bool, so you know when grabbed with 2 hands

    

    public void setLeftHit()
    {
        leftHit = !leftHit;

    }
    public void setRightHit()
    {
        rightHit = !rightHit;

    }
    override protected void checkVR()
    {
        isVR = true;
    }

    override protected void handleInput()
    {
        // Handle VR input LEFT
        Vector2 timeInputVal = timeController.action.ReadValue<Vector2>();


        float playPauseBool = togglePlay.action.ReadValue<float>();
        float Rtrackpad = RtrackClick.action.ReadValue<float>();
        RtriggerVal = RtriggerZ.action.ReadValue<float>();

        // Handle VR input RIGHT
        Vector2 spaceInputVal = spaceController.action.ReadValue<Vector2>();
        float toggleVal = toggleTransparent.action.ReadValue<float>();
        float returnE = NextReturn.action.ReadValue<float>();
        float Ltrackpad = LtrackClick.action.ReadValue<float>();
        LtriggerVal = LtriggerZ.action.ReadValue<float>();

        //Debug.Log("VRindex=  "+ index);

        if (rightHit == true && RtriggerVal >= triggerSens)
        {

            Renderer rightCtrl = rightHTC.GetComponent<MeshRenderer>();

            if (RjustGrabbed)
            {
                initialHandPosition1 = rightController.transform.position;
                initialHandPosition2 = leftController.transform.position;
                initialObjectRotation = scaleSphere.transform.rotation;
                initialObjectScale =  scaleSphere.transform.localScale;
                initialObjectDirection = scaleSphere.transform.position - (initialHandPosition1 + initialHandPosition2) * 0.5f;
                initialObjectDirectionRight = scaleSphere.transform.position - initialHandPosition1;

                //scaleCubeDist = scaleSphere.transform.position - rightController.transform.position;
                //scaleCubeRotation = scaleSphere.transform.eulerAngles - rightController.transform.eulerAngles;
            }

            RjustGrabbed = false;



            Vector3 currentHandPosition1 = rightController.transform.position; // current first hand position
            Vector3 currentHandPosition2 = leftController.transform.position; // current second hand position

            Vector3 handDir1 = (initialHandPosition1 - initialHandPosition2).normalized; // direction vector of initial first and second hand position
            Vector3 handDir2 = (currentHandPosition1 - currentHandPosition2).normalized; // direction vector of current first and second hand position 

            Quaternion handRot = Quaternion.FromToRotation(handDir1, handDir2); // calculate rotation based on those two direction vectors


            float currentGrabDistance = Vector3.Distance(currentHandPosition1, currentHandPosition2);
            float initialGrabDistance = Vector3.Distance(initialHandPosition1, initialHandPosition2);
            float p = (currentGrabDistance / initialGrabDistance); // percentage based on the distance of the initial positions and the new positions


            if (rightHit == true && RtriggerVal >= triggerSens && leftHit == true && LtriggerVal >= triggerSens)
            {
                Vector3 newScale = new Vector3(p * initialObjectScale.x, p * initialObjectScale.y, p * initialObjectScale.z); // calculate new object scale with p
                scaleSphere.transform.localScale = newScale; // set new scale
                scaleSphere.transform.rotation = handRot * initialObjectRotation; // add rotation
                scaleSphere.transform.position = (0.5f * (currentHandPosition1 + currentHandPosition2)) + (handRot * (initialObjectDirection * p));
            }
            else
            {
                scaleSphere.transform.position = rightController.transform.position + initialObjectDirectionRight;
                Quaternion rotDiff = Quaternion.Inverse(scaleSphere.transform.rotation) * rightController.transform.rotation;
                scaleSphere.transform.rotation = scaleSphere.transform.rotation * rotDiff;
                //scaleSphere.transform.rotation = rightController.transform.rotation * initialObjectRotation; // add rotation
                //scaleSphere.transform.position = currentHandPosition1 + (rightController.transform.rotation * (initialObjectDirectionRight));
            }



            // set the position of the object to the center of both hands based on the original object direction relative to the new scale and rotation





            //scaleSphere.transform.eulerAngles = scaleSphere.transform.eulerAngles + scaleCubeRotation;
            //scaleSphere.transform.position = rightController.transform.position + scaleCubeDist;
            rightCtrl.enabled = false;
        }
        else
        {
            Renderer rightCtrl = rightHTC.GetComponent<MeshRenderer>();
            rightCtrl.enabled = true;
            RjustGrabbed = true;

        }

        if(leftHit == true && LtriggerVal >= triggerSens )
        {
            //Debug.Log("Right Grab");
            //Debug.Log("RIGHT");
            Renderer leftCtrl = leftHTC.GetComponent<MeshRenderer>();

            if (LjustGrabbed)
            {
                //scaleCubeDist = scaleSphere.transform.position - leftController.transform.position;
                //Debug.Log(Time.time);
            }
            LjustGrabbed = false;

            //scaleSphere.transform.position = leftController.transform.position + scaleCubeDist;
            leftCtrl.enabled = false;
        }
        else
        {
            Renderer leftCtrl = rightHTC.GetComponent<MeshRenderer>();
            leftCtrl.enabled = true;
            LjustGrabbed = true;

        }

        /*
        if (rightHit == true && RtriggerVal >= triggerSens && LtriggerVal < triggerSens)
        {
            //Debug.Log("Right Grab");
            //Debug.Log("RIGHT");
            
            Renderer rightCtrl = rightHTC.GetComponent<MeshRenderer>();
            
            if (RjustGrabbed)
            {
                scaleCubeDist = scaleSphere.transform.position - rightController.transform.position;
                scaleCubeRotation = scaleSphere.transform.eulerAngles - rightController.transform.eulerAngles;

                Debug.Log(Time.time);
            }
            RjustGrabbed = false;
            scaleSphere.transform.eulerAngles = scaleSphere.transform.eulerAngles + scaleCubeRotation;
            scaleSphere.transform.position = rightController.transform.position + scaleCubeDist;
            rightCtrl.enabled = false;
        }
        else
        {
            Renderer rightCtrl = rightHTC.GetComponent<MeshRenderer>();
            rightCtrl.enabled = true;
            RjustGrabbed = true;

        }

        i
        */


        //Debug.Log(Ltrackpad);
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


        if (returnE > 0.8f && !transparencyMarkerEnabled) //If we got input to toggle transparency
        {
            float timeRes = Time.time - timeStart;

            string res = recordingIndex + "," + markerSphere.transform.position.x + "," + markerSphere.transform.position.y + "," + markerSphere.transform.position.z + "," + markerSphere.transform.localScale.x + "," + timeRes + "," + nrMoveSteps + "," + nrMarkerSteps + "," + nrTimeSteps;
            using (StreamWriter writer = new StreamWriter("ResultVR.txt", true))
            {
                writer.WriteLine(res);
            }
            init();
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
                visualiseData();

                index++;
                if (FrameStuff[0])
                {
                    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
                }
            }

            /*
            else if (timeInputVal.y < RdownSens && index > 0) // If there is input to reduce the playback speed
            {
                rewind = true;
                forward = false;
                visualiseData();

                index--;
                if (FrameStuff[0])
                {
                    FrameStuff[0].text = "Frame: " + index + " / " + maxFileSize;
                }
                rewind = false;
                forward = true;
            }*/
        }
 



        if (timeInputVal != Vector2.zero && Rtrackpad == 1 && preRtrackpad == 0)
        {
            currTime = Time.time;
        }

        if (timeInputVal != Vector2.zero && Rtrackpad == 1) // If there is input controlling the playback
        {
            
            
            //Debug.Log("TIME");

            if (timeInputVal.x > RrightSens && Time.time > currTime + 0.1 && index < maxFileSize - 10) // If there is input to reduce the playback speed
            {
                //Debug.Log("INNIT");
                visualiseData();
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
                visualiseData();
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


        if (markerSphere.activeSelf == false)
        {

            markerSphere.SetActive(true);
            markerSphere.transform.position = leftController.transform.position + leftController.transform.rotation * new Vector3(0, 0, 0.05f);
            transparencyMarkerEnabled = false;
            ToggleMarkerTransparency();
        }
        if (transparencyMarkerEnabled)
        {
            
            markerSphere.transform.position = leftController.transform.position + leftController.transform.rotation * markerDist;
        }
        //Debug.Log(spaceInputVal);
        if (spaceInputVal != Vector2.zero && preLtrackpad == 0 && Ltrackpad == 1) // If there is input controlling the playback
        {
            //Debug.Log("1111111111111");
            if (spaceInputVal.y > LupSens)
            {
                //Debug.Log("222222222");

                ToggleMarkerTransparency();
            }
        }



        if (spaceInputVal != Vector2.zero && Ltrackpad == 1) // If there is input controlling the playback //&& preLtrackpad == 0
        {


            if (spaceInputVal.x > LrightSens)
            {
                
                markerSphere.transform.localScale += 0.0005f * new Vector3(1, 1, 1);
                
            }
            else if (spaceInputVal.x < LleftSens)
            {
                markerSphere.transform.localScale -= 0.0005f * new Vector3(1, 1, 1);
            }

            if (markerSphere.transform.localScale.x < 0.001f)
            {

                markerSphere.transform.localScale = new Vector3(0.001f, 0.001f, 0.00f);
            }
            else if (markerSphere.transform.localScale.x > markerSizeMax)
            {
                markerSphere.transform.localScale = new Vector3(markerSizeMax, markerSizeMax, markerSizeMax);
            }



        }

        preLtrackpad = Ltrackpad;




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

    void ToggleMarkerTransparency()
    {
        Renderer markerRendererR = markerSphere.GetComponent<Renderer>();
        if (!transparencyMarkerEnabled)
        {
            //markerDist = this.transform.position - marker.transform.position;
            //marker.transform.position = this.transform.position + transform.rotation * markerDist;
            markerDist = leftController.transform.position - markerSphere.transform.position;
            markerDist = new Vector3(0, 0, markerDist.magnitude);
            markerTransparent();
            transparencyMarkerEnabled = true;
        }
        else if (transparencyMarkerEnabled)
        {
            markerSolid();
            transparencyMarkerEnabled = false;

        }
    }


    void markerTransparent()
    {
        Renderer skullRendererE = markerSphere.GetComponent<Renderer>();
        skullRendererE.material = transparentMat;
    }
    void markerSolid()
    {
        Renderer skullRendererE = markerSphere.GetComponent<Renderer>();
        skullRendererE.material = solidMat;
    }


}
