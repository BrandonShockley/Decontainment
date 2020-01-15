using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleSlider : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = Time.timeScale;
    }

    public void ChangeTimeScale(float value)
    {
        Time.timeScale = value;
    }
}
