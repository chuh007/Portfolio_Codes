using System;
using _01Scripts.Core.EventSystem;
using UnityEngine;

namespace _01Scripts.UI
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameObject uiPrefab;
        
        private void Awake()
        {
            uiChannel.AddListener<ESCUIEvent>(HandleESCUIEvent);
        }

        private void Start()
        {
            uiPrefab.SetActive(false);
        }

        private void HandleESCUIEvent(ESCUIEvent evt)
        {
            if (uiPrefab == null)
            {
                uiPrefab = FindAnyObjectByType<SettingUI>(FindObjectsInactive.Include).gameObject;
            }
            uiPrefab.SetActive(evt.isOn);
        }
    }
}