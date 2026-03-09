using System;
using Code.Core.EventSystems;
using UnityEngine;

namespace Code.Environments
{
    public class CameraPanTrigger : MonoBehaviour
    {
        public PanDirection panDirection;
        public float panDistance = 3f;
        public float panTime = 0.35f;

        [SerializeField] private GameEventChannelSO cameraChannel;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SendPanEvent(false);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
                SendPanEvent(true);
        }

        private void SendPanEvent(bool isRewind)
        {
            PanEvent evt = CameraEvents.PanEvent;
            evt.panTime = panTime;
            evt.distance = panDistance;
            evt.direction = panDirection;
            evt.isRewindToStart = isRewind;
            
            cameraChannel.RaiseEvent(evt);
        }
    }
}