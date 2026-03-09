using System;
using UnityEngine;
using UnityEngine.UI;

namespace _01Scripts.UI
{
    public enum SliderType
    {
        Master,
        SFX,
        BGM
    }
    
    public class SoundSlider : MonoBehaviour
    {
        [SerializeField] private SliderType sliderType;
        [SerializeField] private Slider slider;
        [SerializeField] private string soundName;
        
        private void Start()
        {
            float value = PlayerPrefs.GetFloat(soundName, 1f);
            slider.value = value;
            slider.onValueChanged.AddListener((v) =>
            {
                switch (sliderType)
                {
                    case SliderType.Master:
                        SoundManager.Instance.SetMasterVolume(v);
                        break;
                    case SliderType.SFX:
                        SoundManager.Instance.SetSFXVolume(v);
                        break;
                    case SliderType.BGM:
                    SoundManager.Instance.SetBGMVolume(v);
                        break;
                }

            });
        }
    }
}