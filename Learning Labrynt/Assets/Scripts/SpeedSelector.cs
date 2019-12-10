using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedSelector : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI valueText;

    public float GetInverse()
    {        
        return 1 / slider.value;
    }

    public float GetSliderValue()
    {
        return slider.value;
    }

    public void UpdateSliderValueText()
    {
        valueText.text = slider.value.ToString();
    }
   
}
