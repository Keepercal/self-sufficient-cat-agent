using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider statusBar;
    public Image statusBarFill;

    private float previousValue;

    public void SetMaxValue(float statusValue)
    {
        statusBar.maxValue = statusValue;
        statusBar.value = statusValue;
    }

    public void SetValue(float statusValue)
    {
        // Change the colour of the fill if the value is increasing
        if (statusValue > previousValue)
        {
            statusBarFill.color = Color.blue;
        }
        // Change the colour of the fill if the value is below 25%
        else if (statusValue < statusBar.maxValue * 0.25f)
        {
            statusBarFill.color = Color.red;
        }
        else if (statusValue < statusBar.maxValue * 0.50f)
        {
            statusBarFill.color = Color.yellow;
        }
        else
        {
            statusBarFill.color = Color.green;
        }

        statusBar.value = statusValue;

        previousValue = statusValue;

    }
    
}
