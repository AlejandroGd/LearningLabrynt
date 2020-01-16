using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValueSelector : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] float defaultValue = 1;

    private void Start()
    {
        slider.value = defaultValue;
        UpdateSliderValueText();
    }

    public float GetInverse()
    {
        if (slider.value == 0) return 1; //Avoid 0 division. This function will be used for the framerate only.
        return 1 / slider.value;
    }

    public float GetSliderValue()
    {
        return slider.value;
    }

    //This is just to keep speed between 1 and the slider max value
    public void UpdateSliderValueText()
    {
        valueText.text = slider.value.ToString();
    }
}
