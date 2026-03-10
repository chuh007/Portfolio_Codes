using System;
using Code.Core.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class PauseUIManager : MonoBehaviour
    {
        public GameEventChannelSO uiEvt;
        public Image pauseImage;
        
        private bool isActive = false;
        private float originalTimeScale = 1f;

        private void Awake()
        {
            uiEvt.AddListener<EscEvent>(HandlePause);
            pauseImage.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            uiEvt.RemoveListener<EscEvent>(HandlePause);
        }

        private void HandlePause(EscEvent evt)
        {
            isActive = !isActive;
            if (isActive)
            {
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = originalTimeScale;
            }
            pauseImage.gameObject.SetActive(isActive);
        }
    }
}