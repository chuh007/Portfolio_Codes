using Code.Core.EventSystems;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Environments
{
    public class CametaSwapTrigger : MonoBehaviour
    {
        public CinemachineCamera leftCamera;
        public CinemachineCamera rightCamera;

        [SerializeField] private GameEventChannelSO cameraChannel;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(leftCamera is null || rightCamera is null) return;
            if (collision.CompareTag("Player"))
            {
                Vector2 exitDirection = (collision.transform.position - transform.position).normalized;

                SwapCameraEvent swapEvt = CameraEvents.SwapCameraEvent;
                swapEvt.leftCamera = leftCamera;
                swapEvt.rightCamera = rightCamera;
                swapEvt.moveDirection = exitDirection;

                cameraChannel.RaiseEvent(swapEvt);
            }
        }
    }
}

