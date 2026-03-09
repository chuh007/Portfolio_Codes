using System;
using TransitionsPlus;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.PSB.Code.Managers;

namespace Work.CUH.Code.UI
{
    public class StageSelectionUI : MonoBehaviour
    {
        [SerializeField] private TransitionAnimator transition;
        [SerializeField] private GameObject stageUI; // 님이 만든 스테이지 UI 넣으셈
        private string _loadSceneName;
        
        private void Awake()
        {
            if(transition == null) transition = GetComponent<TransitionAnimator>();
            if (stageUI == null) Debug.LogError("Stage Ui is null!");
            Bus<OpenBookUIEvent>.OnEvent += HandleOpenBook;
        }

        private void OnDestroy()
        {
            Bus<OpenBookUIEvent>.OnEvent -= HandleOpenBook;
        }

        private void HandleOpenBook(OpenBookUIEvent evt)
        {
            _loadSceneName = evt.LoadSceneName;
            if(stageUI) stageUI.SetActive(true);
            
            Bus<CursorToggleEvent>.Raise(new CursorToggleEvent(true, null));
        }
        
        [ContextMenu("TestLoad")]
        public void LoadGameScene()
        {
            Bus<CursorToggleEvent>.Raise(new CursorToggleEvent(false));
            
            transition.gameObject.SetActive(true);
            transition.sceneNameToLoad = _loadSceneName;
        }
        
        [ContextMenu("TestClose")]
        public void CloseUI()
        {
            Bus<CursorToggleEvent>.Raise(new CursorToggleEvent(false));
            
            Bus<CloseBookUIEvent>.Raise(new CloseBookUIEvent());
            //if(stageUI) stageUI.SetActive(false);
        }
        
    }
}