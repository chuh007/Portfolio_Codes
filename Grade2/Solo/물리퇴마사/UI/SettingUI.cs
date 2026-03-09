using System;
using System.Collections;
using _01Scripts.Core.EventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01Scripts.UI
{
    public class SettingUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        
        [SerializeField] private string saveDataKey = "savedGame";
        private string SaveFilePath => System.IO.Path.Combine(Application.persistentDataPath, saveDataKey + ".json");

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void GotoTitle()
        {
            uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
            StartCoroutine(StartFade());
        }

        private IEnumerator StartFade()
        {
            Debug.Log("Starting Fade");
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