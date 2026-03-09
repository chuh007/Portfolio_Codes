using System;
using System.Collections.Generic;
using _01Scripts.Core;
using _01Scripts.Core.EventSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace _01Scripts.Manager
{

    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO cameraChannel;
        [Header("Camera")]
        [SerializeField] private CinemachineCamera uiSelectCamera; // 기본 카메라
        [SerializeField] private CinemachineCamera uiAttackSelectCamera; // 공격을 고를 때의 카메라
        [SerializeField] private CinemachineCamera uiChoseTargetCamera; // 대상을 고를 때의 카메라
        [SerializeField] private CinemachineCamera uiQTEInputCamera; // QTE를 할 때의 카메라
        [SerializeField] private CinemachineCamera uiUseItemSelectCamera; // 아이템을 사용할 때의 카메라
        [SerializeField] private CinemachineCamera uiBlockInputCamera; // 방어 시의 카메라

        private Dictionary<ControlUIType, CinemachineCamera> _cameras;
        private void Awake()
        {
            _cameras = new Dictionary<ControlUIType, CinemachineCamera>();
            _cameras.Add(ControlUIType.UISelect, uiSelectCamera);
            _cameras.Add(ControlUIType.UIAttackSelect, uiAttackSelectCamera);
            _cameras.Add(ControlUIType.UIChoseTarget, uiChoseTargetCamera);
            _cameras.Add(ControlUIType.UIQTEInput, uiQTEInputCamera);
            _cameras.Add(ControlUIType.UIUseItemSelect, uiUseItemSelectCamera);
            _cameras.Add(ControlUIType.UIBlockInput, uiBlockInputCamera);
            cameraChannel.AddListener<CameraChangeEvent>(HandleCameraChange);
        }

        private void HandleCameraChange(CameraChangeEvent evt)
        {
            var selectCamera = _cameras.GetValueOrDefault(evt.CameraType);
            foreach (var cam in _cameras)
            {
                cam.Value.Priority = 0;
            }

            _cameras.GetValueOrDefault(evt.CameraType).Priority = 10;
        }

        private void OnDestroy()
        {
            cameraChannel.RemoveListener<CameraChangeEvent>(HandleCameraChange);
        }
    }
}