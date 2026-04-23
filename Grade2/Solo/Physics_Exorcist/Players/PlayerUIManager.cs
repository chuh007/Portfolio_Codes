using System;
using System.Collections.Generic;
using _01Scripts.Core;
using _01Scripts.Core.EventSystem;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace _01Scripts.Players
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO playerChannel;
        
        [Header("Camera")]
        [SerializeField] private GameObject uiSelectObj;
        [SerializeField] private GameObject uiAttackSelectObj;
        [SerializeField] private GameObject uiChoseTargetObj;
        [SerializeField] private GameObject uiQTEInputObj;
        [SerializeField] private GameObject uiUseItemSelectObj;
        [SerializeField] private GameObject uiBlockInputObj;
        
        private Dictionary<ControlUIType, GameObject> _uiObjects;
        
        private void Awake()
        {
            _uiObjects = new Dictionary<ControlUIType, GameObject>();
            _uiObjects.Add(ControlUIType.UISelect, uiSelectObj);
            _uiObjects.Add(ControlUIType.UIAttackSelect, uiAttackSelectObj);
            _uiObjects.Add(ControlUIType.UIChoseTarget, uiChoseTargetObj);
            _uiObjects.Add(ControlUIType.UIQTEInput, uiQTEInputObj);
            _uiObjects.Add(ControlUIType.UIUseItemSelect, uiUseItemSelectObj);
            _uiObjects.Add(ControlUIType.UIBlockInput, uiBlockInputObj);
            playerChannel.AddListener<PlayerUIChangeEvent>(HandleUIChange);
        }

        private void HandleUIChange(PlayerUIChangeEvent evt)
        {
            foreach (var obj in _uiObjects)
            {
                obj.Value.SetActive(false);
            }
            if(evt.ControlUIType == default) return;
            var selectCamera = _uiObjects.GetValueOrDefault(evt.ControlUIType);
            selectCamera.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            playerChannel.RemoveListener<PlayerUIChangeEvent>(HandleUIChange);
        }
    }
}