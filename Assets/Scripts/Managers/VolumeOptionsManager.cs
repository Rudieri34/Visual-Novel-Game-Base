using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeOptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _volumeSlider;

    private void Start()
    {
        SetMasterVolume();
    }

    public void SetMasterVolume()
    { 
        float volume = _volumeSlider.value;
        _audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
}
