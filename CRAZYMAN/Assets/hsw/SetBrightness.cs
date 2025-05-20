using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class SetBrightness : MonoBehaviour
{
    public Slider brightnessSlider;

    public PostProcessProfile brightnessProfile;
    public PostProcessLayer layer;

    private  AutoExposure autoExposure;
    // Start is called before the first frame update
    void Start()
    {
        if(brightnessProfile.TryGetSettings(out autoExposure))
            AdjustBrightness(brightnessSlider.value);
        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustBrightness(float value)
    {
        //float exposureValue = Mathf.Lerp(0.11f, 2f, (sliderValue - 1f) / 99f);//0.11f 1.10f
        if(autoExposure != null)
        {
            autoExposure.keyValue.Override(value);
        }         
    }
}