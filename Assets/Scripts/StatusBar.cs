using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxValue(int statusValue)
    {
        slider.maxValue = statusValue;
        slider.value = statusValue;
    }

    public void SetValue(int statusValue)
    {
        slider.value = statusValue;
    }
    
}
