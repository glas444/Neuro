using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slid : MonoBehaviour
{
    public Slider slide;
    public bool sliderSelecte = false;

    // Start is called before the first frame update
    void Start()
    {
        slide = GetComponent<Slider>();
    }

    public void SliderSelected()
    {
        Debug.Log("Se");
        sliderSelecte = true;
    }

    public void SliderDeselect()
    {
        Debug.Log("De");
        sliderSelecte = false;
    }

    public void Update()
    {

    }
}