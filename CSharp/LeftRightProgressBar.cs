using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftRightProgressBar : MonoBehaviour
{
    public Slider _leftBar;
    public Slider _rightBar;


    void Awake()
    {
        _leftBar.value = 0f;
        _rightBar.value = 0f;
    }

    public void LeftRightBar_Value(float l_value, float r_value)
    {
        _leftBar.value = l_value;
        _rightBar.value = r_value;
    }
}
