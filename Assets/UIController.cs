using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController singleton;

    public Slider grabSuccessRateSlider;

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSuccessRateSlider(float newSuccessRate)
    {
        grabSuccessRateSlider.value = newSuccessRate / 100;
    }
}
