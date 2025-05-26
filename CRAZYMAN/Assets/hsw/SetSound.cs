using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SetSound : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioMixer masterMixer;
    //public Slider audioSlider;
    public void AudioControl(float sound)
    {
        //sound = audioSlider.value;
        if (sound == -40f)
        {
            masterMixer.SetFloat("TempSound1", -80);//d음소거 위해서 -80
            masterMixer.SetFloat("TempSound2", -80);
        }
        else
        {
            masterMixer.SetFloat("TempSound1", sound);
            masterMixer.SetFloat("TempSound2", sound);
        }
    }
}
