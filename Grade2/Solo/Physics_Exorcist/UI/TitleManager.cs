using System;
using System.Collections;
using _01Scripts.Core.EventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01Scripts.UI
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameObject settingPanel;
        [SerializeField] private AudioClip titleBGM;
        private bool isSettingPanelOpen = false;

        [SerializeField] private string saveDataKey = "savedGame";
        private string SaveFilePath => System.IO.Path.Combine(Application.persistentDataPath, saveDataKey + ".json");

        
        private void Awake()
        {
            settingPanel.SetActive(false);
        }

        private void Start()
        {
            int width = 1920;
            int height = 1080;

            Screen.SetResolution(width, height, false);
            SoundManager.Instance.PlayBGM(titleBGM);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenSetting();
            }
        }

        public void StartGame()
        {
            if (System.IO.File.Exists(SaveFilePath))
                System.IO.File.Delete(SaveFilePath);
            uiChannel.AddListener<FadeCompleteEvent>(HandleFadeComplete);
            StartCoroutine(StartFade());
        }
        
        private IEnumerator StartFade()
        {
            FadeEvent fadeEvt = UIEvents.FadeEvent;
            fadeEvt.isFadeIn = false;
            fadeEvt.fadeTime = 0.5f;
            yield return new WaitForSeconds(0.5f);
            uiChannel.RaiseEvent(fadeEvt);
        }

        private void HandleFadeComplete(FadeCompleteEvent obj)
        {
            SoundManager.Instance.StopBGM();
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeComplete);
            SceneManager.LoadScene("SerachScene");
        }
        
        public void OpenSetting()
        {
            isSettingPanelOpen = !isSettingPanelOpen;
            settingPanel.SetActive(isSettingPanelOpen);
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}
