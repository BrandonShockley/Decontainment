using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = AudioListener.volume;
        slider.onValueChanged.AddListener(HandleValueChanged);
    }

    private void HandleValueChanged(float newValue)
    {
        AudioListener.volume = newValue;
    }
}
