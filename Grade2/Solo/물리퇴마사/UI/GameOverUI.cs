using System;
using System.Collections;
using _01Scripts.Core.EventSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _01Scripts.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private TextMeshProUGUI text;
        
        private void Awake()
        {
            uiChannel.AddListener<DeadUIEvent>(HandleDeadUIEvent);
            gameOverText.alpha = 0;
            text.alpha = 0;
            gameObject.SetActive(false);
        }

        private void HandleDeadUIEvent(DeadUIEvent obj)
        {
            gameObject.SetActive(true);
            gameOverText.DOFade(1, 0.5f).OnComplete(() =>
            {
                text.DOFade(1, 0.1f);
            });
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<DeadUIEvent>(HandleDeadUIEvent);
        }

        public void GotoTitle()
        {
            uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
            StartCoroutine(StartFade());
        }

        private IEnumerator StartFade()
        {
            Time.timeScale = 1;
            FadeEvent fadeEvt = UIEvents.FadeEvent;
            fadeEvt.isFadeIn = false;
            fadeEvt.fadeTime = 0.5f;
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.StopBGM();
            uiChannel.RaiseEvent(fadeEvt);
        }

        
        private void HandleFadeComplete(FadeCompleteEvent obj)
        {
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeComplete);
            SceneManager.LoadScene("TitleScene");
        }
        
        public void CloseGame()
        {
            Application.Quit();
        }
    }
}