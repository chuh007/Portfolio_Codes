using System;
using Code.Core.EventSystems;
using Code.Entities;
using Code.Players;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Environments
{
    public enum PortalType
    {
        SceneChange, Teleport
    }
    
    public class Portal : MonoBehaviour
    {
        [SerializeField] private PortalType portalType;
        [SerializeField] private string nextSceneName;
        [SerializeField] private GameEventChannelSO uiChannel;

        private bool _isTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered && portalType == PortalType.SceneChange) return;

            if (other.CompareTag("Player"))
            {
                _isTriggered = true;
                Player player = other.GetComponent<Player>();
                
                player.GetCompo<EntityMover>().CanManualMove = false;

                FadeEvent fadeEvt = UIEvents.FadeEvent;
                fadeEvt.isFadeIn = false;
                fadeEvt.fadeTime = 0.5f;
                
                uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
                uiChannel.RaiseEvent(fadeEvt);
                
            }
        }

        private void HandleFadeComplete(FadeCompleteEvent fadeCompleteEvt)
        {
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeComplete); //이벤트 제거후 씬변환.
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}