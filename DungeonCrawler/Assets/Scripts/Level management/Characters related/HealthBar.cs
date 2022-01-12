using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        Debug.Assert(slider, "HealthBar: slider component not found");
    }

    public void SetHealthBar(float value)
    {
        slider.value = value;
    }
}
