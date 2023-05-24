using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  Script which enables changing orientation of the observer (rotates camera
  around itself) using the mouse in the desktop setting. Adjust xSens and ySens
  inside Unity to change the sensitivity (how much the camera rotates based on
  mouse movements).
*/

public class LookDirection : MonoBehaviour
{
    public GameObject marker;
    public Material transparentMat;
    public Material solidMat;
    public bool leftRot;

    //protected bool transparencyEnabled;

    public float xSens;
    public float ySens;
    float xRot;
    float yRot;

    private Quaternion targetRot;
    private Quaternion currentRot;
    private Vector3 dirToMark;
    private float deltacam;
    public float slerpSmoothValue = 0.3f;
    //private bool startrot;

    public Vector3 markerDist;
    private bool scrolltoggle;
    public float markerSizeMin;
    public float markerSizeMax;

    public float startScaleMarker;
    public GameObject cameraLineStart;
    public GameObject cameraLineEnd;

    public BaseSimulator SliderFancy;

    public Transform lookDir;
    public Vector3 predir;
    public Vector3 observerPosition;

    public float moveSpeed;

    public float angleMax = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
        
        scrolltoggle = true;
        //startrot = true;
        marker.transform.localScale = new Vector3(startScaleMarker, startScaleMarker, startScaleMarker);
        marker.transform.position = this.transform.position + transform.rotation * markerDist;
        ToggleTransparency();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        observerPosition = lookDir.transform.position;
    }



    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButton(0) == false)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SliderFancy.nrMoveSteps++;
            }
            if (Input.GetKey(KeyCode.W))
            {
                observerPosition += lookDir.forward * moveSpeed / 50;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                SliderFancy.nrMoveSteps++;
            }
            if (Input.GetKey(KeyCode.S))
            {
                observerPosition -= lookDir.forward * moveSpeed / 50;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                SliderFancy.nrMoveSteps++;
            }
            if (Input.GetKey(KeyCode.A))
            {
                observerPosition -= lookDir.right * moveSpeed / 50;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                SliderFancy.nrMoveSteps++;
            }
            if (Input.GetKey(KeyCode.D))
            {
                observerPosition += lookDir.right * moveSpeed / 50;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SliderFancy.nrMoveSteps++;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                observerPosition.y -= moveSpeed / 50;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                SliderFancy.nrMoveSteps++;
            }
            if (Input.GetKey(KeyCode.E))
            {
                observerPosition.y += moveSpeed / 50;
            }
        }


        lookDir.transform.position = observerPosition;


        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed = 0.07f;
        }
        else
        {
            moveSpeed = 0.01f;
        }


        if (Input.GetMouseButtonDown(1) & Input.GetMouseButton(0) == false)
        {
            SliderFancy.nrMoveSteps++;

            //lookDir.rotation = this.transform.rotation;
            //lookDir.position = this.transform.position;
        }
        //RIGHTMOUSEBUTTON
        if (Input.GetMouseButton(1) & Input.GetMouseButton(0) == false)
        {
            SliderFancy.INXrotateCam++;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

            xRot -= mouseY;
            yRot += mouseX;
            //Debug.Log(xRot);

            // limits PoV up/down
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            // rotate cam
            lookDir.rotation = Quaternion.Euler(xRot, yRot, 0);

        }
        else if (Input.GetMouseButtonUp(1) & Input.GetMouseButton(0) == false)
        {
            //Debug.Log(xRot);
            Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.None;
        }

        
        if (Input.GetMouseButtonDown(0) & SliderFancy.transparencyMarkerEnabled == false & Input.GetMouseButton(1) == false && !SliderFancy.sliderSelecte)
        {
            SliderFancy.nrMoveSteps++;

            markerDist = this.transform.position - marker.transform.position;
            lookDir.transform.LookAt(marker.transform.position);
        }

        //LEFTMOUSEBUTTON
        if (Input.GetMouseButton(0) & SliderFancy.transparencyMarkerEnabled == false & Input.GetMouseButton(1) == false && !SliderFancy.sliderSelecte )// && marker.activeSelf == true)
        {
            SliderFancy.INXrotateMark++;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lookDir.transform.position = marker.transform.position - (lookDir.transform.forward * markerDist.magnitude);
            lookDir.transform.RotateAround(marker.transform.position, Vector3.up, Input.GetAxis("Mouse X"));
            lookDir.transform.RotateAround(marker.transform.position, -lookDir.transform.right, Input.GetAxis("Mouse Y"));

            observerPosition = lookDir.transform.position;
        }
        else if (Input.GetMouseButtonUp(0) & SliderFancy.transparencyMarkerEnabled == false & Input.GetMouseButton(1) == false)
        {
            xRot = lookDir.eulerAngles.x;
            yRot = lookDir.eulerAngles.y;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        
        if (Input.GetMouseButtonDown(2))
        {
            SliderFancy.nrMarkerSteps++;
            SliderFancy.INXpickplaceMark++;
            if (marker.activeSelf == false)
            {
                SliderFancy.transparencyMarkerEnabled = false;
            }
            ToggleTransparency();
        }

        if (SliderFancy.transparencyMarkerEnabled)
        {
            marker.transform.position = this.transform.position + transform.rotation * markerDist;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            scrolltoggle = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            scrolltoggle = true;
        }

        if (scrolltoggle)
        {
            markerDist.z += Input.mouseScrollDelta.y * 0.001f;
            //if (markerDist.z < 0.005)
            //{
            //markerDist.z = 0.005f;
            //}

        }
        else
        {
            marker.transform.localScale += Input.mouseScrollDelta.y * 0.001f * new Vector3(1, 1, 1);

            
            if (marker.transform.localScale.x < markerSizeMin)
            {
                
                marker.transform.localScale = new Vector3(markerSizeMin, markerSizeMin, markerSizeMin);
            } 
            else if (marker.transform.localScale.x > markerSizeMax)
            {
                marker.transform.localScale = new Vector3(markerSizeMax, markerSizeMax, markerSizeMax);
            }

            
        }


        //-------------------------------------------------
        /*
        private void PlayAudioClip(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }
        
		//-------------------------------------------------
		private void PlayPointerHaptic( bool validLocation )
		{
			if ( pointerHand != null )
			{
				if ( validLocation )
				{
					pointerHand.TriggerHapticPulse( 800 );
				}
				else
				{
					pointerHand.TriggerHapticPulse( 100 );
				}
			}
		}
        */




        //else if (Input.GetMouseButtonUp(2))
        //{
        //    ToggleTransparency();
        //}
    }
    void ToggleTransparency()
    {
        Renderer markerRenderer = marker.GetComponent<Renderer>();
        if (!SliderFancy.transparencyMarkerEnabled)
        {
            //markerDist = this.transform.position - marker.transform.position;
            //marker.transform.position = this.transform.position + transform.rotation * markerDist;
            markerDist = this.transform.position - marker.transform.position;
            //markerDist = new Vector3(-0.03f, -0.01f, markerDist.magnitude);
            markerDist = new Vector3(0, 0, markerDist.magnitude);
            markerTransparent();
            SliderFancy.transparencyMarkerEnabled = true;
        }
        else if (SliderFancy.transparencyMarkerEnabled)
        {
            markerSolid();
            SliderFancy.transparencyMarkerEnabled = false;

        }
    }

    void markerTransparent()
    {
        Renderer skullRenderer = marker.GetComponent<Renderer>();
        skullRenderer.material = transparentMat;
    }
    void markerSolid()
    {
        Renderer skullRenderer = marker.GetComponent<Renderer>();
        skullRenderer.material = solidMat;
    }
}
