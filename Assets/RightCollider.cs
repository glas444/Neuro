using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightCollider : MonoBehaviour
{
    public VRSimulator VRscript;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Brain")
        {
            VRscript.setRightHit();

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Brain")
        {
            VRscript.setRightHit();
        }
    }
}
